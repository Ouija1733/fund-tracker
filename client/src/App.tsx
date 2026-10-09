import { useState } from "react";
import { FundList } from "./components/FundList";
import { CreateFundForm } from "./components/CreateFundForm";
import { FundDetail } from "./components/FundDetail";

export default function App() {
    const [selectedId, setSelectedId] = useState<number | null>(null);

    return (
        <main className="layout">
            <header>
                <h1>FundTracker</h1>
                <p className="muted">Investment funds, NAV history and flows</p>
            </header>
            <div className="grid">
                <div className="stack">
                    <FundList
                        selectedId={selectedId}
                        onSelect={setSelectedId}
                    />
                    <CreateFundForm />
                </div>
                {selectedId !== null ? (
                    <FundDetail fundId={selectedId} />
                ) : (
                    <section className="card">
                        <p className="muted">
                            Select a fund to see its NAV history.
                        </p>
                    </section>
                )}
            </div>
        </main>
    );
}
