import { useState, type ChangeEvent, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../api/client";

const emptyForm = { name: "", isin: "", currency: "EUR" };

export function CreateFundForm() {
    const queryClient = useQueryClient();
    const [form, setForm] = useState(emptyForm);

    const mutation = useMutation({
        mutationFn: api.createFund,
        onSuccess: () => {
            setForm(emptyForm);
            queryClient.invalidateQueries({ queryKey: ["funds"] });
        },
    });

    function update(field: keyof typeof emptyForm) {
        return (e: ChangeEvent<HTMLInputElement>) =>
            setForm({ ...form, [field]: e.target.value });
    }

    function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault();
        mutation.mutate({
            name: form.name.trim(),
            isin: form.isin.trim().toUpperCase(),
            currency: form.currency.trim().toUpperCase(),
        });
    }

    return (
        <section className="card">
            <h2>New fund</h2>
            <form onSubmit={handleSubmit} className="form">
                <label>
                    Name
                    <input
                        value={form.name}
                        onChange={update("name")}
                        required
                        minLength={2}
                    />
                </label>
                <label>
                    ISIN
                    <input
                        value={form.isin}
                        onChange={update("isin")}
                        required
                        maxLength={12}
                    />
                </label>
                <label>
                    Currency
                    <input
                        value={form.currency}
                        onChange={update("currency")}
                        required
                        maxLength={3}
                    />
                </label>
                <button type="submit" disabled={mutation.isPending}>
                    {mutation.isPending ? "Saving…" : "Create fund"}
                </button>
                {mutation.isError && (
                    <p role="alert" className="error">
                        {mutation.error.message}
                    </p>
                )}
                {mutation.isSuccess && <p className="ok">Fund created.</p>}
            </form>
        </section>
    );
}
