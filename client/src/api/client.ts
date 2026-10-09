import type { CreateFund, Fund, NavPoint, PagedResult } from "./types";

type Problem = { title?: string; errors?: Record<string, string[]> };

async function request<T>(url: string, init?: RequestInit): Promise<T> {
    const res = await fetch(url, {
        ...init,
        headers: { "Content-Type": "application/json", ...init?.headers },
    });

    if (!res.ok) {
        const problem: Problem | null = await res.json().catch(() => null);
        const firstValidationError = problem?.errors
            ? Object.values(problem.errors).flat()[0]
            : undefined;
        throw new Error(
            firstValidationError ??
                problem?.title ??
                `Request failed with status ${res.status}`,
        );
    }

    return res.status === 204 ? (undefined as T) : ((await res.json()) as T);
}

export const api = {
    searchFunds: (search: string, page: number, signal?: AbortSignal) =>
        request<PagedResult<Fund>>(
            `/api/funds?search=${encodeURIComponent(search)}&page=${page}&pageSize=10`,
            { signal },
        ),
    getFund: (id: number, signal?: AbortSignal) =>
        request<Fund>(`/api/funds/${id}`, { signal }),
    navHistory: (id: number, signal?: AbortSignal) =>
        request<NavPoint[]>(`/api/funds/${id}/nav-history`, { signal }),
    createFund: (body: CreateFund) =>
        request<Fund>("/api/funds", {
            method: "POST",
            body: JSON.stringify(body),
        }),
};
