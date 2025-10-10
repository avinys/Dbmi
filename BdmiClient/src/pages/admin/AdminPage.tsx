import { useEffect, useState } from "react";
import { Link, Outlet } from "react-router-dom";

const LINKS = [
	{ to: "/admin/movies", label: "Movies" },
	{ to: "/admin/reviews", label: "Reviews" },
	{ to: "/admin/genres", label: "Genres" },
	{ to: "/admin/users", label: "Users" },
];

export default function AdminPage() {
	const [open, setOpen] = useState(false);

	// Close on Esc + lock body scroll while open
	useEffect(() => {
		const onKey = (e: KeyboardEvent) =>
			e.key === "Escape" && setOpen(false);
		document.addEventListener("keydown", onKey);
		document.body.style.overflow = open ? "hidden" : "";
		return () => {
			document.removeEventListener("keydown", onKey);
			document.body.style.overflow = "";
		};
	}, [open]);

	return (
		<div>
			{/* Top bar */}
			<header className="border-b">
				<div className="container flex items-center justify-between py-3">
					<div className="text-xl font-medium">Admin</div>

					{/* Desktop nav */}
					<nav className="hidden md:flex flex-wrap justify-center gap-4">
						{LINKS.map((l) => (
							<Link key={l.to} className="btn-ghost" to={l.to}>
								{l.label}
							</Link>
						))}
					</nav>

					{/* Mobile hamburger */}
					<button
						className="md:hidden btn-ghost"
						aria-expanded={open}
						aria-controls="admin-mobile-drawer"
						onClick={() => setOpen((o) => !o)}
					>
						☰
					</button>
				</div>
			</header>

			{/* Mobile drawer */}
			<div
				className={`fixed inset-0 z-50 md:hidden ${
					open ? "" : "pointer-events-none"
				}`}
			>
				{/* overlay */}
				<div
					className={`absolute inset-0 bg-black/40 transition-opacity ${
						open ? "opacity-100" : "opacity-0"
					}`}
					onClick={() => setOpen(false)}
				/>
				{/* panel */}
				<div
					id="admin-mobile-drawer"
					role="dialog"
					aria-modal="true"
					className={`absolute left-0 top-0 h-full w-64 bg-surface-100 border-r border-surface-200 p-4 transition-transform
                      ${open ? "translate-x-0" : "-translate-x-full"}`}
				>
					<nav className="flex flex-col gap-3">
						{LINKS.map((l) => (
							<Link
								key={l.to}
								to={l.to}
								className="btn-ghost"
								onClick={() => setOpen(false)}
							>
								{l.label}
							</Link>
						))}
					</nav>
				</div>
			</div>

			{/* Page content */}
			<main className="container py-6">
				<Outlet />
			</main>
		</div>
	);
}
