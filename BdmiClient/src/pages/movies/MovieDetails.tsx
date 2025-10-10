// src/pages/movies/MovieDetails.tsx
import { useParams, useNavigate } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Movie, Review, Genre, CreateUpdateMovieDto } from "@/lib/types";
import { useState, useMemo } from "react";
import Modal from "@/components/Modal";
import { useAuth } from "@/auth/useAuth";
import { ReviewForm } from "./ReviewForm";
import { MovieForm } from "./MovieForm";
import ConfirmDelete from "@/components/ConfirmDelete";

export default function MovieDetails() {
	const { id } = useParams();
	const mid = Number(id);
	const nav = useNavigate();
	const q = useQueryClient();
	const { user } = useAuth();

	// Movie
	const { data: movie } = useQuery<Movie>({
		queryKey: ["movie", mid],
		enabled: Number.isFinite(mid),
		queryFn: async () => (await api.get(`/api/movies/${mid}`)).data,
	});

	// Genres (for names + edit form)
	const { data: allGenres = [] } = useQuery<Genre[]>({
		queryKey: ["genres"],
		queryFn: async () => (await api.get("/api/genres")).data,
	});

	// Reviews
	const { data: dto } = useQuery<{ movieId: number; reviews: Review[] }>({
		queryKey: ["reviews", mid],
		enabled: Number.isFinite(mid),
		queryFn: async () =>
			(await api.get(`/api/movies/${mid}/reviews?includeText=true`)).data,
	});

	// ----- Review modal state -----
	const [openReview, setOpenReview] = useState(false);
	const [editingReview, setEditingReview] = useState<Review | null>(null);
	const [openDeleteReview, setOpenDeleteReview] = useState(false);
	const [deletingReview, setDeletingReview] = useState<Review | null>(null);

	// ----- Movie edit modal state (admin) -----
	const [openMovie, setOpenMovie] = useState(false);

	const [openDeleteMovie, setOpenDeleteMovie] = useState(false);
	const [deletingMovie, setDeletingMovie] = useState<Movie | null>(null);

	// ----- Mutations -----
	const saveReview = useMutation({
		mutationFn: async (payload: Review) => {
			if (payload.id) {
				return (await api.put(`/api/reviews/${payload.id}`, payload))
					.data;
			}
			return (await api.post(`/api/reviews`, payload)).data;
		},
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["reviews", mid] });
			q.invalidateQueries({ queryKey: ["movie", mid] }); // refresh avg score
			setOpenReview(false);
			setEditingReview(null);
		},
	});

	const delReview = useMutation({
		mutationFn: async (rid: number) =>
			(await api.delete(`/api/reviews/${rid}`)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["reviews", mid] });
			q.invalidateQueries({ queryKey: ["movie", mid] });
		},
	});

	const approveMovie = useMutation({
		mutationFn: async () =>
			(await api.put(`/api/movies/approve/${mid}`)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["movie", mid] });
			q.invalidateQueries({ queryKey: ["movies-all"] });
		},
	});

	const rejectMovie = useMutation({
		mutationFn: async () =>
			(await api.put(`/api/movies/reject/${mid}`)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["movie", mid] });
			q.invalidateQueries({ queryKey: ["movies-all"] });
		},
	});

	const updateMovie = useMutation({
		mutationFn: async (dto: CreateUpdateMovieDto) =>
			(await api.put(`/api/movies/${mid}`, dto)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["movie", mid] });
			q.invalidateQueries({ queryKey: ["movies-all"] });
			setOpenMovie(false);
		},
	});

	const deleteMovie = useMutation({
		mutationFn: async (movieId: number) =>
			(await api.delete(`/api/movies/${movieId}`)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["movies-all"] });
			nav("/"); // or nav("/admin/movies") if you prefer
		},
	});

	const genreNames = useMemo(
		() =>
			(movie?.genreIds ?? [])
				.map((gid) => allGenres.find((g) => g.id === gid)?.name)
				.filter(Boolean)
				.join(", "),
		[movie, allGenres]
	);

	return (
		<div className="space-y-6">
			{movie && (
				<div className="card">
					<div className="grid gap-4 md:grid-cols-2">
						<div className="mx-auto w-full max-w-3xl">
							<img
								src={
									movie.posterUrl ||
									`https://picsum.photos/seed/${movie.id}/800/450`
								}
								alt={movie.title}
								className="rounded-xl block w-full h-auto object-contain"
							/>
						</div>
						<div>
							<h1 className="text-2xl font-bold mb-2">
								{movie.title}
							</h1>
							<p className="text-gray-300 mb-3">
								{movie.description}
							</p>
							{user?.role === "Admin" && (
								<div className="text-sm mb-1">
									<span className="opacity-80">Status: </span>
									<strong>{statusLabel(movie.status)}</strong>
								</div>
							)}
							<div className="text-sm mb-1">
								<span className="opacity-80">Year: </span>
								<strong>{movie.releaseYear ?? "—"}</strong>
							</div>
							<div className="text-sm mb-1">
								<span className="opacity-80">Duration: </span>
								<strong>{movie.durationMin ?? "—"} min</strong>
							</div>
							<div className="text-sm mb-1">
								<span className="opacity-80">Genres: </span>
								<strong>{genreNames || "—"}</strong>
							</div>
							<div className="text-sm">
								<span className="opacity-80">
									Average score:{" "}
								</span>
								<strong>
									{movie.averageScore == null
										? "—"
										: Number(movie.averageScore).toFixed(1)}
								</strong>
							</div>
						</div>
					</div>
				</div>
			)}

			{/* Admin actions */}
			{user?.role === "Admin" && movie && (
				<div className="card flex flex-wrap justify-center gap-2">
					{movie.status !== 1 ? (
						<button
							onClick={() => approveMovie.mutate()}
							className="btn-primary"
							disabled={approveMovie.isPending}
						>
							{approveMovie.isPending ? "Approving…" : "Approve"}
						</button>
					) : (
						<button
							onClick={() => rejectMovie.mutate()}
							className="btn-ghost text-red-500"
							disabled={rejectMovie.isPending}
						>
							{rejectMovie.isPending ? "Rejecting…" : "Reject"}
						</button>
					)}

					<button
						className="btn-ghost"
						onClick={() => setOpenMovie(true)}
						disabled={updateMovie.isPending}
					>
						Edit
					</button>

					<button
						className="btn-ghost text-red-500"
						onClick={() => {
							setOpenDeleteMovie(true);
							setDeletingMovie(movie);
						}}
						disabled={deleteMovie.isPending}
					>
						Delete
					</button>
				</div>
			)}

			{/* Reviews */}
			<div className="card">
				<div className="flex items-center justify-between mb-3">
					<h2 className="text-xl font-semibold">Reviews</h2>
					{user && movie && (
						<button
							className="btn-primary"
							onClick={() => {
								setEditingReview({
									id: 0,
									userId: user.id,
									movieId: mid,
									score: 10,
									body: "",
								} as Review);
								setOpenReview(true);
							}}
						>
							Write a review
						</button>
					)}
				</div>

				<ul className="space-y-3">
					{dto?.reviews?.map((r) => (
						<li
							key={r.id}
							className="p-3 rounded-xl bg-surface-100 border border-surface-200"
						>
							<div className="flex flex-col items-center justify-between text-center sm:flex-row">
								<div>
									<div className="flex flex-col sm:flex-row sm:gap-4">
										{user?.role === "Admin" &&
											r.userId === user.id && (
												<p>*Author*</p>
											)}
										<p className="font-semibold">
											{r.title}
										</p>
										<p>{r.score}</p>
									</div>
									{r.body && (
										<p className="text-gray-300 mt-1">
											{r.body}
										</p>
									)}
								</div>
								{user &&
									(user.role === "Admin" ||
										user.id === r.userId) && (
										<div className="flex gap-2">
											<button
												className="btn-ghost"
												onClick={() => {
													setEditingReview(r);
													setOpenReview(true);
												}}
											>
												Edit
											</button>
											<button
												className="btn-ghost"
												onClick={() => {
													setDeletingReview(r);
													setOpenDeleteReview(true);
													console.log(r);
												}}
											>
												Delete
											</button>
										</div>
									)}
							</div>
						</li>
					))}
				</ul>
			</div>

			{/* Review modal */}
			<Modal
				open={openReview}
				onClose={() => {
					setOpenReview(false);
					setEditingReview(null);
				}}
				title={editingReview?.id ? "Edit review" : "New review"}
			>
				{editingReview && (
					<ReviewForm
						value={editingReview}
						onSubmit={(v) => saveReview.mutate(v)}
					/>
				)}
			</Modal>

			{/* Delete Review Modal */}
			<Modal
				open={openDeleteReview}
				onClose={() => {
					setOpenDeleteReview(false);
					setDeletingReview(null);
				}}
				title="Confirm Delete Review"
			>
				{deletingReview && (
					<ConfirmDelete
						review={deletingReview}
						onDelete={delReview.mutate}
					/>
				)}
			</Modal>

			{/* Movie edit modal (admin) */}
			<Modal
				open={openMovie}
				onClose={() => setOpenMovie(false)}
				title="Edit movie"
			>
				{movie && (
					<MovieForm
						initial={movieToDto(movie)}
						genres={allGenres}
						onSubmit={(dto) => updateMovie.mutate(dto)}
					/>
				)}
			</Modal>

			{/* Delete Movie Modal */}
			<Modal
				open={openDeleteMovie}
				onClose={() => {
					setOpenDeleteMovie(false);
					setDeletingMovie(null);
				}}
				title="Confirm Delete Movie"
			>
				{deletingMovie && (
					<ConfirmDelete
						movie={deletingMovie}
						onDelete={deleteMovie.mutate}
					/>
				)}
			</Modal>
		</div>
	);
}

function statusLabel(s: 0 | 1 | 2) {
	return s === 1 ? "Approved" : s === 2 ? "Rejected" : "Pending";
}

function movieToDto(m: Movie): CreateUpdateMovieDto {
	return {
		title: m.title,
		description: m.description ?? "",
		genreIds: m.genreIds ?? [],
		releaseYear: m.releaseYear ?? null,
		durationMin: m.durationMin ?? null,
	};
}
