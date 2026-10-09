import random
import sys

random.seed(42)
n = int(sys.argv[1]) if len(sys.argv) > 1 else 50000
isins = ["LU2000000013", "LU2000000021", "LU2000000039"]

with open("transactions.csv", "w", newline="") as f:
    f.write("ExternalId,Isin,InvestorName,Type,Amount,TradeDate\n")
    for i in range(1, n + 1):
        isin = random.choice(isins)
        kind = "Redemption" if random.random() < 0.25 else "Subscription"
        amount = f"{random.uniform(500, 250000):.2f}"
        date = f"2026-{random.randint(7, 9):02d}-{random.randint(1, 28):02d}"
        if i % 100 == 0:
            isin = "LU2000000014"  # cifra di controllo sbagliata
        if i % 150 == 0:
            amount = "abc"         # importo non valido
        line = f"CSV-{i:06d},{isin},Investor {random.randint(1, 2000)},{kind},{amount},{date}\n"
        f.write(line)
        if i % 499 == 0:
            f.write(line)          # riga duplicata

print(f"Written {n} rows to transactions.csv")