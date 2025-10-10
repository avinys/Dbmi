import { Review } from "@/lib/types";
import { useState } from "react";

export function ReviewForm({
	value,
	onSubmit,
}: {
	value: Review;
	onSubmit: (v: Review) => void;
}) {
	const [form, setForm] = useState(value);
	return (
		<form
			className="space-y-3"
			onSubmit={(e) => {
				e.preventDefault();
				onSubmit(form);
			}}
		>
			<div>
				<div>
					<label className="label">Title</label>
					<input
						className="input"
						type="text"
						value={form.title || ""}
						onChange={(e) =>
							setForm({ ...form, title: e.target.value })
						}
					/>
				</div>
				<label className="label">Score: {form.score}</label>
				<input
					type="range"
					min={1}
					max={10}
					value={form.score}
					onChange={(e) =>
						setForm({ ...form, score: Number(e.target.value) })
					}
					className="w-full"
				/>
			</div>
			<div>
				<label className="label">Content</label>
				<textarea
					className="input h-28"
					value={form.body || ""}
					onChange={(e) => setForm({ ...form, body: e.target.value })}
				/>
			</div>
			{/* userId and movieId are required by backend */}
			<input type="hidden" value={form.userId} />
			<input type="hidden" value={form.movieId} />
			<button className="btn-primary">Save</button>
		</form>
	);
}
