import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Movie, Genre } from "@/lib/types";
import { Link } from "react-router-dom";
import { useMemo, useState } from "react";
import { useAuth } from "@/auth/useAuth";

export default function Home() {
	const { user } = useAuth();
	const { data: movies } = useQuery<Movie[]>({
		queryKey: ["movies"],
		queryFn: async () => (await api.get("/api/movies")).data,
	});
	const { data: genres } = useQuery<Genre[]>({
		queryKey: ["genres"],
		queryFn: async () => (await api.get("/api/genres")).data,
	});

	const [q, setQ] = useState("");
	const [genreId, setGenreId] = useState<number | "">("");
	const [minScore, setMinScore] = useState(0);

	const filtered = useMemo(() => {
		let list = movies ?? [];
		if (q)
			list = list.filter((m) =>
				m.title.toLowerCase().includes(q.toLowerCase())
			);
		if (genreId !== "")
			list = list.filter((m) => m.genreIds.includes(genreId));
		if (minScore > 0)
			list = list.filter((m) => (m.averageScore ?? 0) >= minScore);
		return list;
	}, [movies, q, genreId, minScore]);

	return (
		<div className="space-y-6">
			<div className="card">
				<div className="grid gap-3 md:grid-cols-4">
					<input
						className="input"
						placeholder="Search by title"
						value={q}
						onChange={(e) => setQ(e.target.value)}
					/>
					<select
						className="input"
						value={genreId}
						onChange={(e) =>
							setGenreId(
								e.target.value ? Number(e.target.value) : ""
							)
						}
					>
						<option value="">All genres</option>
						{genres?.map((g) => (
							<option key={g.id} value={g.id}>
								{g.name}
							</option>
						))}
					</select>
					<div>
						<label className="label">Min score: {minScore}</label>
						<input
							className="w-full"
							type="range"
							min={0}
							max={10}
							value={minScore}
							onChange={(e) =>
								setMinScore(Number(e.target.value))
							}
						/>
					</div>
					<Link
						to="/account/movies"
						className="btn-primary justify-center"
					>
						Upload movie
					</Link>
				</div>
			</div>

			<div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
				{filtered?.map((m) => (
					<Link
						to={`/movies/${m.id}`}
						key={m.id}
						className="card hover:border-primary"
					>
						<div className="aspect-[16/9] overflow-hidden rounded-xl mb-3">
							{user?.role === "Admin" && (
								<span
									className={`p-1 relative left-2 top-8 rounded-xl ${
										m.status === 0
											? "bg-orange-300"
											: m.status === 1
											? "bg-green-300"
											: "bg-red-300"
									}`}
								>
									{m.status === 0
										? "Pending"
										: m.status === 1
										? "Approved"
										: "Rejected"}
								</span>
							)}
							<img
								src={
									m.posterUrl ||
									`https://picsum.photos/seed/${m.id}/800/450`
								}
								alt={m.title}
								className="w-full h-full object-cover"
							/>
						</div>
						<div className="flex items-center justify-between">
							<h3 className="font-semibold">{m.title}</h3>
							<span className="text-sm text-gray-300">
								⭐ {m.averageScore ?? "—"}
							</span>
						</div>
					</Link>
				))}
			</div>
		</div>
	);
}
