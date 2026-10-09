import { useQuery } from "@tanstack/react-query";
import { api } from "../api/client";

function changeClass(change: number | null): string {
    if (change === null || change === 0) return "num";
    return change > 0 ? "num up" : "num down";
}

export function FundDetail({ fundId }: { fundId: number }) {
    const fund = useQuery({
        queryKey: ["fund", fundId],
        queryFn: ({ signal }) => api.getFund(fundId, signal),
    });
    const history = useQuery({
        queryKey: ["nav-history", fundId],
        queryFn: ({ signal }) => api.navHistory(fundId, signal),
    });

    if (fund.isError || history.isError) {
        return (
            <section className="card">
                <p role="alert" className="error">
                    {(fund.error ?? history.error)?.message}
                </p>
            </section>
        );
    }

    if (!fund.data || !history.data) {
        return (
            <section className="card">
                <p>Loading…</p>
            </section>
        );
    }

    return (
        <section className="card">
            <h2>{fund.data.name}</h2>
            <p className="muted">
                {fund.data.isin} · {fund.data.currency}
            </p>
            <table>
                <thead>
                    <tr>
                        <th>Date</th>
                        <th className="num">NAV</th>
                        <th className="num">Change</th>
                    </tr>
                </thead>
                <tbody>
                    {[...history.data].reverse().map((point) => (
                        <tr key={point.date}>
                            <td>{point.date}</td>
                            <td className="num">{point.value.toFixed(4)}</td>
                            <td className={changeClass(point.changePercent)}>
                                {point.changePercent === null
                                    ? "—"
                                    : `${point.changePercent.toFixed(2)}%`}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </section>
    );
}
