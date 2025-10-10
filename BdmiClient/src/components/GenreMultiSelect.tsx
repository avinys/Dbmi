// src/components/GenreMultiSelect.tsx
import { useMemo, useState } from "react";
import type { Genre } from "@/lib/types";
import { X } from "lucide-react";

export default function GenreMultiSelect({
	all,
	value,
	onChange,
}: {
	all: Genre[];
	value: number[];
	onChange: (ids: number[]) => void;
}) {
	const [q, setQ] = useState("");

	const selected = useMemo(
		() =>
			value
				.map((id) => all.find((g) => g.id === id))
				.filter(Boolean) as Genre[],
		[value, all]
	);

	const suggestions = useMemo(() => {
		const lower = q.trim().toLowerCase();
		return all
			.filter((g) => !value.includes(g.id))
			.filter((g) =>
				lower ? g.name.toLowerCase().includes(lower) : false
			)
			.slice(0, 8);
	}, [all, value, q]);

	const add = (id: number) => onChange([...new Set([...value, id])]);
	const remove = (id: number) => onChange(value.filter((v) => v !== id));

	return (
		<div className="space-y-2">
			<div className="flex flex-wrap gap-2">
				{selected.map((g) => (
					<span
						key={g.id}
						className="inline-flex items-center gap-1 rounded-xl bg-surface-200 px-2 py-1 text-sm"
					>
						{g.name}
						<button
							type="button"
							aria-label={`Remove ${g.name}`}
							className="opacity-80 hover:opacity-100"
							onClick={() => remove(g.id)}
						>
							<X className="w-4 h-4" />
						</button>
					</span>
				))}
				{selected.length === 0 && (
					<span className="text-sm text-gray-400">
						No genres selected
					</span>
				)}
			</div>

			<input
				className="input"
				placeholder="Search genres…"
				value={q}
				onChange={(e) => setQ(e.target.value)}
			/>

			{suggestions.length > 0 && (
				<div className="grid grid-cols-2 sm:grid-cols-3 gap-2 ">
					{suggestions.map((g) => (
						<button
							key={g.id}
							type="button"
							className="btn-ghost justify-start border-1 border-slate-500"
							onClick={() => {
								add(g.id);
								setQ("");
							}}
						>
							{g.name}
						</button>
					))}
				</div>
			)}
		</div>
	);
}
