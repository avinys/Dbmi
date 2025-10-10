import GenreMultiSelect from "@/components/GenreMultiSelect";
import { CreateUpdateMovieDto, Genre } from "@/lib/types";
import { useMemo, useState } from "react";

export function MovieForm({
	initial,
	genres,
	onSubmit,
}: {
	initial: CreateUpdateMovieDto;
	genres: Genre[];
	onSubmit: (dto: CreateUpdateMovieDto) => void;
}) {
	const [form, setForm] = useState<CreateUpdateMovieDto>(initial);
	const [imgOk, setImgOk] = useState<boolean>(true);

	function normalizeUrl(u?: string | null): string | null {
		const s = (u ?? "").trim();
		if (!s) return null;
		if (/^https?:\/\//i.test(s)) return s;
		// if user pasted without scheme, assume https
		return `https://${s}`;
	}

	function looksLikeUrl(u?: string | null): boolean {
		const s = (u ?? "").trim();
		if (!s) return true; // empty allowed
		try {
			// tolerate missing scheme when typing
			const candidate = /^https?:\/\//i.test(s) ? s : `https://${s}`;
			new URL(candidate);
			return true;
		} catch {
			return false;
		}
	}

	const posterPreview = useMemo(() => {
		const s = (form.posterUrl ?? "").trim();
		if (!s) return null;
		return /^https?:\/\//i.test(s) ? s : `https://${s}`;
	}, [form.posterUrl]);

	return (
		<form
			className="space-y-3 overflow-auto"
			onSubmit={(e) => {
				e.preventDefault();
				onSubmit({
					...form,
					posterUrl: normalizeUrl(form.posterUrl), // send null or https URL
				});
			}}
		>
			<div>
				<label className="label">Title</label>
				<input
					className="input"
					value={form.title}
					onChange={(e) =>
						setForm({ ...form, title: e.target.value })
					}
				/>
			</div>

			<div>
				<label className="label">Description</label>
				<textarea
					className="input h-28"
					value={form.description ?? ""}
					onChange={(e) =>
						setForm({ ...form, description: e.target.value })
					}
				/>
			</div>

			{/* Poster URL + Preview */}
			<div className="space-y-2">
				<label className="label">Poster URL (optional)</label>
				<input
					className="input"
					type="url"
					placeholder="https://example.com/poster.jpg"
					value={form.posterUrl ?? ""}
					onChange={(e) => {
						setForm({ ...form, posterUrl: e.target.value });
						setImgOk(true); // reset on change
					}}
					onBlur={(e) => {
						// normalize on blur (optional)
						const normalized = normalizeUrl(e.target.value);
						setForm({ ...form, posterUrl: normalized ?? "" });
					}}
				/>
				{!looksLikeUrl(form.posterUrl) && (
					<p className="text-sm text-red-400">
						Please enter a valid URL.
					</p>
				)}

				{posterPreview && (
					<div className="mt-2">
						{imgOk ? (
							<img
								src={posterPreview}
								alt={`${form.title || "Poster"} preview`}
								className="rounded-xl max-h-64 w-auto"
								onError={() => setImgOk(false)}
							/>
						) : (
							<p className="text-sm text-yellow-400">
								Couldn’t load the image from this URL.
							</p>
						)}
					</div>
				)}
			</div>

			<div className="space-y-2">
				<label className="label">Genres</label>
				<GenreMultiSelect
					all={genres}
					value={form.genreIds}
					onChange={(ids) => setForm({ ...form, genreIds: ids })}
				/>
			</div>

			<div className="grid grid-cols-2 gap-3">
				<div>
					<label className="label">Release year</label>
					<input
						className="input"
						type="number"
						min={1888}
						max={2100}
						value={form.releaseYear ?? ""}
						onChange={(e) =>
							setForm({
								...form,
								releaseYear: Number(e.target.value) || null,
							})
						}
					/>
				</div>
				<div>
					<label className="label">Duration (min)</label>
					<input
						className="input"
						type="number"
						min={1}
						max={600}
						value={form.durationMin ?? ""}
						onChange={(e) =>
							setForm({
								...form,
								durationMin: Number(e.target.value) || null,
							})
						}
					/>
				</div>
			</div>

			<button className="btn-primary">Save</button>
		</form>
	);
}
