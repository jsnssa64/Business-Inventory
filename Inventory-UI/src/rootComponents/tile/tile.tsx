import React from "react";

export default function Tile({ title, description, footer, children }: { title: string; description: string; footer: string; imageUrl: string; children?: React.ReactNode }) {
    return (
        <>
            <div className="tile bg-blue-200 rounded-2xl p-4 m-4 shadow-lg">
                <div className="tile-header">
                    <h3 className="tile-title">{title}</h3>
                    <p className="tile-description">{description}</p>
                </div>
                <div className="tile-content">
                    <p>{children}</p>
                </div>
                <div className="tile-footer">
                    <p>{footer}</p>
                </div>
            </div>
        </>
    );
}