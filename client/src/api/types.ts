export type Fund = {
    id: number;
    name: string;
    isin: string;
    currency: string;
    latestNav: number | null;
    latestNavDate: string | null;
};

export type PagedResult<T> = {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
};

export type NavPoint = {
    date: string;
    value: number;
    changePercent: number | null;
};

export type CreateFund = {
    name: string;
    isin: string;
    currency: string;
};
