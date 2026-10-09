using System.Diagnostics;
using FundTracker.Api.Data;
using FundTracker.Api.Import;
using FundTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FundTracker.Api.Services;

public record RowError(int Line, string Message);

public record ImportResultDto(int TotalRows, int Imported, int Duplicates,
    int ErrorCount, IReadOnlyList<RowError> FirstErrors);

public interface IImportService
{
    Task<ImportResultDto> ImportTransactionsAsync(Stream csv, CancellationToken ct);
}

public class ImportService(AppDbContext db, ILogger<ImportService> logger) : IImportService
{
    private const int BatchSize = 1000;

    public async Task<ImportResultDto> ImportTransactionsAsync(Stream csv, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();

        // 1. Una sola query per tutti i fondi, invece di una per riga
        var fundIdsByIsin = await db.Funds.AsNoTracking().ToDictionaryAsync(f => f.Isin, f => f.Id, ct);

        // 2. Lettura riga per riga in streaming e validazione
        var errors = new List<RowError>();
        var valid = new List<ParsedTransaction>();
        var total = 0;

        using var reader = new StreamReader(csv);
        var header = await reader.ReadLineAsync(ct);
        if (header?.Trim() != TransactionCsvParser.ExpectedHeader)
            return new ImportResultDto(0, 0, 0, 1,
                [new RowError(1, $"Header must be: {TransactionCsvParser.ExpectedHeader}")]);

        var lineNumber = 1;
        while (await reader.ReadLineAsync(ct) is { } line)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;
            total++;

            var (row, error) = TransactionCsvParser.ParseLine(line);
            if (row is null)
                errors.Add(new RowError(lineNumber, error!));
            else if (!fundIdsByIsin.ContainsKey(row.Isin))
                errors.Add(new RowError(lineNumber, $"No fund with ISIN {row.Isin}."));
            else
                valid.Add(row);
        }

        // 3. Salvataggio a blocchi, saltando i duplicati (idempotenza)
        var seen = new HashSet<string>();
        var imported = 0;
        var duplicates = 0;

        foreach (var batch in valid.Chunk(BatchSize))
        {
            var ids = batch.Select(r => r.ExternalId).ToList();
            var existing = (await db.Transactions
                    .Where(t => ids.Contains(t.ExternalId))
                    .Select(t => t.ExternalId)
                    .ToListAsync(ct))
                .ToHashSet();

            var toInsert = new List<Transaction>();
            foreach (var r in batch)
            {
                if (existing.Contains(r.ExternalId) || !seen.Add(r.ExternalId))
                {
                    duplicates++;
                    continue;
                }

                toInsert.Add(new Transaction
                {
                    ExternalId = r.ExternalId,
                    FundId = fundIdsByIsin[r.Isin],
                    InvestorName = r.InvestorName,
                    Type = r.Type,
                    Amount = r.Amount,
                    TradeDate = r.TradeDate
                });
            }

            db.Transactions.AddRange(toInsert);
            await db.SaveChangesAsync(ct);
            db.ChangeTracker.Clear();
            imported += toInsert.Count;
        }

        logger.LogInformation(
            "Import finished: {Total} rows, {Imported} imported, {Duplicates} duplicates, {Errors} errors in {Elapsed} ms",
            total, imported, duplicates, errors.Count, stopwatch.ElapsedMilliseconds);

        return new ImportResultDto(total, imported, duplicates, errors.Count, errors.Take(100).ToList());
    }
}