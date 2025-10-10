import { Navigate, Route, Routes } from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { Header } from "@/components/layout/Header";
import { Footer } from "@/components/layout/Footer";
import Home from "@/pages/Home";
import Login from "@/pages/auth/Login";
import Register from "@/pages/auth/Register";
import MovieDetails from "@/pages/movies/MovieDetails";
import MyReviews from "@/pages/account/MyReviews";
import MyMovies from "@/pages/account/MyMovies";
import Profile from "@/pages/account/Profile";
import AdminMovies from "@/pages/admin/AdminMovies";
import AdminGenres from "@/pages/admin/AdminGenres";
import AdminReviews from "@/pages/admin/AdminReviews";
import { AuthProvider } from "@/auth/AuthContext";
import { useAuth } from "@/auth/useAuth";
import AdminPage from "./pages/admin/AdminPage";
import AdminUsers from "./pages/admin/AdminUsers";
import Links from "./pages/Links";
import About from "./pages/About";

export default function App() {
	return (
		<AuthProvider>
			<div>
				<Toaster />
			</div>
			<div className="min-h-screen grid grid-rows-[auto,1fr,auto] text-white">
				<Header />
				<main className="site">
					<div className="container py-6">
						<Routes>
							<Route path="/" element={<Home />} />
							<Route path="/login" element={<Login />} />
							<Route path="/register" element={<Register />} />
							<Route path="/links" element={<Links />} />
							<Route path="/about" element={<About />} />
							<Route
								path="/movies/:id"
								element={<MovieDetails />}
							/>

							<Route
								path="/account/profile"
								element={
									<RequireAuth>
										<Profile />
									</RequireAuth>
								}
							/>
							<Route
								path="/account/reviews"
								element={
									<RequireAuth>
										<MyReviews />
									</RequireAuth>
								}
							/>
							<Route
								path="/account/movies"
								element={
									<RequireAuth>
										<MyMovies />
									</RequireAuth>
								}
							/>

							<Route
								path="/admin"
								element={
									<RequireAdmin>
										<AdminPage />
									</RequireAdmin>
								}
							>
								{/* default child: redirect to movies */}
								<Route
									index
									element={<Navigate to="movies" replace />}
								/>
								<Route
									path="movies"
									element={<AdminMovies />}
								/>
								<Route
									path="reviews"
									element={<AdminReviews />}
								/>
								<Route
									path="genres"
									element={<AdminGenres />}
								/>
								<Route path="users" element={<AdminUsers />} />
								{/* optional admin 404 redirect */}
								<Route
									path="*"
									element={<Navigate to="movies" replace />}
								/>
							</Route>

							<Route
								path="*"
								element={<Navigate to="/" replace />}
							/>
						</Routes>
					</div>
				</main>
				<Footer />
			</div>
		</AuthProvider>
	);
}

function RequireAuth({ children }: { children: React.ReactNode }) {
	const { user } = useAuth();
	if (!user) return <Navigate to="/login" replace />;
	return <>{children}</>;
}
function RequireAdmin({ children }: { children: React.ReactNode }) {
	const { user } = useAuth();
	// console.log(user);
	if (!user) return <Navigate to="/login" replace />;
	if (user.role !== "Admin") return <Navigate to="/" replace />;
	return <>{children}</>;
}
