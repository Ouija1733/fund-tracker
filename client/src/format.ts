export function formatMoney(value: number, currency: string): string {
    return new Intl.NumberFormat("en-GB", {
        style: "currency",
        currency,
    }).format(value);
}
