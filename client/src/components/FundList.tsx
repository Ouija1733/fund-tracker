import { useState } from "react";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { api } from "../api/client";
import { useDebounce } from "../hooks/useDebounce";
import { formatMoney } from "../format";

type Props = {
    selectedId: number | null;
    onSelect: (id: number) => void;
};

export function FundList({ selectedId, onSelect }: Props) {
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const debouncedSearch = useDebounce(search);

    const { data, isPending, isError, error } = useQuery({
        queryKey: ["funds", debouncedSearch, page],
        queryFn: ({ signal }) => api.searchFunds(debouncedSearch, page, signal),
        placeholderData: keepPreviousData,
    });

    const totalPages = data
        ? Math.max(1, Math.ceil(data.totalCount / data.pageSize))
        : 1;

    return (
        <section className="card">
            <h2>Funds</h2>
            <input
                type="search"
                aria-label="Search funds"
                placeholder="Search by name or ISIN"
                value={search}
                onChange={(e) => {
                    setSearch(e.target.value);
                    setPage(1);
                }}
            />

            {isPending && <p>Loading…</p>}
            {isError && (
                <p role="alert" className="error">
                    {error.message}
                </p>
            )}

            {data && data.items.length === 0 && (
                <p className="muted">No funds match your search.</p>
            )}

            {data && data.items.length > 0 && (
                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>ISIN</th>
                            <th className="num">Latest NAV</th>
                        </tr>
                    </thead>
                    <tbody>
                        {data.items.map((fund) => (
                            <tr
                                key={fund.id}
                                className={
                                    fund.id === selectedId
                                        ? "selected"
                                        : undefined
                                }
                                onClick={() => onSelect(fund.id)}
                            >
                                <td>{fund.name}</td>
                                <td>{fund.isin}</td>
                                <td className="num">
                                    {fund.latestNav !== null
                                        ? formatMoney(
                                              fund.latestNav,
                                              fund.currency,
                                          )
                                        : "—"}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <div className="pager">
                <button
                    disabled={page <= 1}
                    onClick={() => setPage((p) => p - 1)}
                >
                    Previous
                </button>
                <span>
                    Page {page} of {totalPages}
                </span>
                <button
                    disabled={page >= totalPages}
                    onClick={() => setPage((p) => p + 1)}
                >
                    Next
                </button>
            </div>
        </section>
    );
}
