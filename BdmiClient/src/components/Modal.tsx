import { X } from "lucide-react";
import type { ReactNode } from "react";

export default function Modal({
	open,
	onClose,
	title,
	children,
}: {
	open: boolean;
	onClose: () => void;
	title: string;
	children: ReactNode;
}) {
	if (!open) return null;
	return (
		<div className="fixed inset-0 z-50 flex items-center justify-center overflow-auto">
			<div className="absolute inset-0 bg-black/60" onClick={onClose} />
			<div className="relative z-10 w-[95%] max-w-lg rounded-2xl bg-surface-100 border border-surface-200 p-5 shadow-xl animate-in fade-in zoom-in-95">
				<div className="flex items-center justify-between mb-3">
					<h3 className="text-lg font-semibold">{title}</h3>
					<button
						aria-label="Close"
						onClick={onClose}
						className="btn-ghost"
					>
						<X className="w-5 h-5" />
					</button>
				</div>
				{children}
			</div>
		</div>
	);
}
