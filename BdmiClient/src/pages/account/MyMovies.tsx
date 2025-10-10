// src/pages/account/MyMovies.tsx
import { useAuth } from "@/auth/useAuth";
import Modal from "@/components/Modal";
import { api } from "@/lib/api";
import { CreateUpdateMovieDto, Genre, Movie } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { MovieForm } from "../movies/MovieForm";
import ConfirmDelete from "@/components/ConfirmDelete";

export default function MyMovies() {
	const { user } = useAuth();
	const q = useQueryClient();

	const { data: allGenres = [] } = useQuery<Genre[]>({
		queryKey: ["genres"],
		queryFn: async () => (await api.get("/api/genres")).data,
	});

	const { data: movies = [] } = useQuery<Movie[]>({
		queryKey: ["my-movies", user?.id],
		queryFn: async () => (await api.get(`/api/users/movies`)).data,
		enabled: !!user,
	});

	const [open, setOpen] = useState(false);
	const [editingMovie, setEditingMovie] = useState<Movie | null>(null);
	const [openDelete, setOpenDelete] = useState<boolean>(false);
	const [deleting, setDeleting] = useState<Movie | null>(null);

	const saveMovie = useMutation({
		mutationFn: async (payload: {
			id?: number;
			dto: CreateUpdateMovieDto;
		}) => {
			if (payload.id) {
				return (await api.put(`/api/movies/${payload.id}`, payload.dto))
					.data;
			}
			return (await api.post(`/api/movies`, payload.dto)).data;
		},
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["my-movies", user?.id] });
			setOpen(false);
			setEditingMovie(null);
		},
	});

	const delMovie = useMutation({
		mutationFn: async (id: number) =>
			(await api.delete(`/api/movies/${id}`)).data,
		onSuccess: () =>
			q.invalidateQueries({ queryKey: ["my-movies", user?.id] }),
	});

	const startCreate = () => {
		setEditingMovie(null);
		setOpen(true);
	};

	const startEdit = (m: Movie) => {
		setEditingMovie(m);
		setOpen(true);
	};

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<h1 className="text-2xl font-bold">My movies</h1>
				<button className="btn-primary" onClick={startCreate}>
					Add movie
				</button>
			</div>

			<ul className="grid gap-3">
				{movies.map((m) => (
					<li
						key={m.id}
						className="card flex flex-col md:flex-row md:items-center md:justify-between gap-3"
					>
						<div>
							<div className="font-semibold">{m.title}</div>
							<div className="text-sm text-gray-400">
								{statusLabel(m.status)} • {m.releaseYear} •{" "}
								{m.durationMin} min
							</div>
							<div className="text-xs text-gray-400">
								{m.genreIds
									.map(
										(id) =>
											allGenres.find((g) => g.id === id)
												?.name
									)
									.filter(Boolean)
									.join(", ")}
							</div>
						</div>
						<div className="flex gap-2">
							<button
								className="btn-ghost"
								onClick={() => startEdit(m)}
							>
								Edit
							</button>
							<button
								className="btn-ghost"
								onClick={() => {
									setOpenDelete(true);
									setDeleting(m);
								}}
							>
								Delete
							</button>
						</div>
					</li>
				))}
			</ul>

			<Modal
				open={open}
				onClose={() => {
					setOpen(false);
					setEditingMovie(null);
				}}
				title={editingMovie ? "Edit movie" : "New movie"}
			>
				<MovieForm
					initial={movieToForm(editingMovie)}
					genres={allGenres}
					onSubmit={(dto) =>
						saveMovie.mutate({ id: editingMovie?.id, dto })
					}
				/>
			</Modal>
			<Modal
				open={openDelete}
				onClose={() => {
					setOpenDelete(false);
					setDeleting(null);
				}}
				title="Confirm Delete Movie"
			>
				{deleting && (
					<ConfirmDelete
						movie={deleting}
						onDelete={delMovie.mutate}
					/>
				)}
			</Modal>
		</div>
	);
}

function statusLabel(s: 0 | 1 | 2) {
	return s === 1 ? "Approved" : s === 2 ? "Rejected" : "Pending";
}

function movieToForm(m?: Movie | null): CreateUpdateMovieDto {
	if (!m) {
		return {
			title: "",
			description: "",
			genreIds: [],
			releaseYear: new Date().getFullYear(),
			durationMin: 90,
		};
	}
	return {
		title: m.title,
		description: m.description,
		genreIds: m.genreIds ?? [],
		releaseYear: m.releaseYear ?? new Date().getFullYear(),
		durationMin: m.durationMin ?? 90,
	};
}
