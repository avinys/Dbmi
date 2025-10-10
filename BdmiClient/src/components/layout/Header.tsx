import { Link, NavLink, useNavigate } from "react-router-dom";
import { Film, Menu, LogOut } from "lucide-react";
import { useState } from "react";
import { useAuth } from "@/auth/useAuth";

export function Header() {
	const [open, setOpen] = useState(false);
	const { user, logout } = useAuth();
	const nav = useNavigate();

	return (
		<header className="site">
			<div className="container flex items-center justify-between py-3">
				<Link to="/" className="flex items-center gap-2 font-bold">
					<Film className="h-6 w-6 text-primary" />
					<span>Bdmi</span>
				</Link>

				{/* Desktop Nav */}
				<nav className="hidden md:flex items-center gap-4">
					<NavLink
						to="/"
						className={({ isActive }) =>
							`hover:text-primary ${
								isActive ? "text-primary" : ""
							}`
						}
					>
						Movies
					</NavLink>
					{user && (
						<NavLink
							to="/account/movies"
							className="hover:text-primary"
						>
							My Movies
						</NavLink>
					)}
					{user && (
						<NavLink
							to="/account/reviews"
							className="hover:text-primary"
						>
							My Reviews
						</NavLink>
					)}
					{user?.role === "Admin" && (
						<NavLink to="/admin" className="hover:text-primary">
							Admin
						</NavLink>
					)}
				</nav>

				<div className="hidden md:flex items-center gap-3">
					{!user ? (
						<>
							<Link to="/login" className="btn-ghost">
								Login
							</Link>
							<Link to="/register" className="btn-primary">
								Register
							</Link>
						</>
					) : (
						<div className="flex items-center gap-3">
							<NavLink
								to="/account/profile"
								className="hover:text-primary"
							>
								{user.username}
							</NavLink>
							<button
								className="btn-ghost"
								onClick={() => {
									logout();
									nav("/");
								}}
							>
								<LogOut className="h-4 w-4" /> Logout
							</button>
						</div>
					)}
				</div>

				{/* Mobile */}
				<button
					className="md:hidden btn-ghost"
					aria-label="Open menu"
					onClick={() => setOpen(!open)}
				>
					<Menu className="h-6 w-6" />
				</button>
			</div>

			{/* Mobile Drawer */}
			{open && (
				<div className="md:hidden border-t border-surface-200 bg-surface-100">
					<div className="container py-3 flex flex-col gap-3">
						<NavLink to="/" onClick={() => setOpen(false)}>
							Movies
						</NavLink>
						{user && (
							<NavLink
								to="/account/movies"
								onClick={() => setOpen(false)}
							>
								My Movies
							</NavLink>
						)}
						{user && (
							<NavLink
								to="/account/reviews"
								onClick={() => setOpen(false)}
							>
								My Reviews
							</NavLink>
						)}
						{user?.role === "Admin" && (
							<NavLink to="/admin" onClick={() => setOpen(false)}>
								Admin
							</NavLink>
						)}
						{!user ? (
							<div className="flex gap-3">
								<Link
									to="/login"
									onClick={() => setOpen(false)}
									className="btn-ghost"
								>
									Login
								</Link>
								<Link
									to="/register"
									onClick={() => setOpen(false)}
									className="btn-primary"
								>
									Register
								</Link>
							</div>
						) : (
							<>
								<NavLink
									to="/account/profile"
									onClick={() => setOpen(false)}
								>
									Profile
								</NavLink>
								<button
									className="btn-ghost w-fit"
									onClick={() => {
										setOpen(false);
										logout();
									}}
								>
									Logout
								</button>
							</>
						)}
					</div>
				</div>
			)}
		</header>
	);
}
