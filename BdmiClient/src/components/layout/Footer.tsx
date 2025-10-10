import { Link } from "react-router-dom";

export function Footer() {
	return (
		<footer className="site">
			<div className=" flex flex-row container justify-around py-6 text-sm text-gray-400  md:grid-cols-2 gap-1">
				<div>
					<Link to="/About" className="font-semibold text-white mb-2">
						About
					</Link>
				</div>
				<div>
					<Link to="/links" className="font-semibold text-white mb-2">
						Links
					</Link>
				</div>
			</div>
		</footer>
	);
}
