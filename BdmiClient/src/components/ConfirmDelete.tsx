import { Movie, Review, UserDetailsDTO } from "@/lib/types";
import { ReactElement } from "react";

function roleLabel(role: unknown): "Admin" | "User" {
	if (typeof role === "number") return role === 1 ? "Admin" : "User";
	if (typeof role === "string")
		return role.trim().toLowerCase() === "admin" ? "Admin" : "User";
	return "User";
}

function ConfirmDelete({
	movie,
	review,
	user,
	onDelete,
}: {
	movie?: Movie;
	review?: Review;
	user?: UserDetailsDTO;
	onDelete: (id: number) => void;
}) {
	// console.log("ConfirmDelete", review, movie, user);
	let id: number;
	let infoElement: ReactElement;
	if (movie) {
		id = movie.id;
		infoElement = (
			<div className="text-left">
				<p>Id: {movie.id}</p>
				<p>Title: {movie.title}</p>
				{movie.description && <p>Description: {movie.description}</p>}
				<p>Average Score: {movie.averageScore ?? 0}</p>
				<p>
					Status:{" "}
					{movie.status === 0
						? "Pending"
						: movie.status === 1
						? "Approved"
						: "Rejected"}
				</p>
			</div>
		);
	}
	if (review) {
		console.log(review);
		id = review.id;
		infoElement = (
			<div className="text-left">
				<p>Id: {review.id}</p>
				<p>Movie Id: {review.movieId}</p>
				{review.title && <p>Title: {review.title}</p>}
				{review.body && <p>Body: {review.body}</p>}
				<p>Score: {review.score}</p>
				<p>Uploaded by user#{review.userId}</p>
			</div>
		);
	}
	if (user) {
		id = user.id;
		infoElement = (
			<div className="text-left">
				<p>Id: {user.id}</p>
				<p>Username: {user.username}</p>
				<p>Email: {user.email}</p>
				<p>Role: {roleLabel(user.role)}</p>
			</div>
		);
	}

	return (
		<div>
			<form
				onSubmit={() => onDelete(id)}
				className="mx-auto w-full text-center"
			>
				{infoElement}
				<button className="w-auto btn-primary bg-red-500/50 text-white hover:bg-red-300">
					Delete
				</button>
			</form>
		</div>
	);
}

export default ConfirmDelete;
