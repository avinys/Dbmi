import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Movie } from "@/lib/types";

export default function AdminMovies() {
	const q = useQueryClient();
	const { data: movies } = useQuery<Movie[]>({
		queryKey: ["movies-all"],
		queryFn: async () => (await api.get("/api/movies")).data,
	});

	const approve = useMutation({
		mutationFn: async (m: Movie) =>
			(await api.post(`/api/movies/approve/${m.id}`)).data,
		onSuccess: () => q.invalidateQueries({ queryKey: ["movies-all"] }),
	});

	const reject = useMutation({
		mutationFn: async (m: Movie) =>
			(await api.post(`/api/movies/reject/${m.id}`)).data,
		onSuccess: () => q.invalidateQueries({ queryKey: ["movies-all"] }),
	});

	return (
		<div className="space-y-4">
			<h1 className="text-2xl font-bold">Admin · Movies</h1>
			<ul className="grid gap-3">
				{movies?.map(
					(m) =>
						m.status !== 1 && (
							<li
								key={m.id}
								className="card flex items-center justify-between"
							>
								<div>
									<div className="font-semibold">
										{m.title}
									</div>
									<div className="text-sm text-gray-400">
										{m.status === 0
											? "Pending"
											: "Rejected"}
									</div>
								</div>
								{m.status === 0 && (
									<div className="flex gap-2">
										<button
											className="btn-primary"
											onClick={() => approve.mutate(m)}
										>
											Approve
										</button>
										<button
											className="btn-ghost"
											onClick={() => reject.mutate(m)}
										>
											Reject
										</button>
									</div>
								)}
								{/* {m.status === 1 && (
									<button
										className="btn-ghost"
										onClick={() => reject.mutate(m)}
									>
										Reject
									</button>
								)} */}
								{m.status === 2 && (
									<button
										className="btn-primary"
										onClick={() => approve.mutate(m)}
									>
										Approve
									</button>
								)}
							</li>
						)
				)}
			</ul>
		</div>
	);
}
