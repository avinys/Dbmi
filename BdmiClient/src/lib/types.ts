export type Role = "User" | "Admin";
export type Genre = { id: number; name: string };
export type Movie = {
	id: number;
	title: string;
	description?: string;
	genreIds: number[];
	averageScore?: number | null;
	status: 0 | 1 | 2;
	uploadedByUserId: number;
	releaseYear?: number | null;
	durationMin?: number | null;
	posterUrl?: string | null;
};

export type CreateUpdateMovieDto = {
	title: string;
	description?: string;
	genreIds: number[];
	releaseYear?: number | null;
	durationMin?: number | null;
	posterUrl?: string | null;
};

export type Review = {
	id: number;
	userId: number;
	movieId: number;
	score: number; // 1..10
	title?: string;
	body?: string;
	createdAt?: string;
};

export type UpdateReviewDTO = {
	score: number;
	title?: string;
	body?: string;
};

export type UserListItemDTo = {
	id: number;
	username: string;
	email: string;
	createdAt: Date;
	role: Role;
};

export type UserDetailsDTO = {
	id: number;
	username: string;
	email: string;
	createdAt: Date;
	role: Role;
};
export type User = {
	id: number;
	username: string;
	email: string;
	createdAt: Date;
	role: Role;
};
