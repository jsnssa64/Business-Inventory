import React, { useState } from "react";

export default function MenuSpan ({ title, children, isVisible, onClick }: { title: string, children?: React.ReactNode, isVisible: boolean, onClick?: () => void }) {
    return <span data-dropdown-id={title}
                className={`menu-dropdown-toggle rounded-none hover:bg-slate-50 ${isVisible ? 'menu-dropdown-show' : ''}`}
                onClick={onClick}>
                    {children}
            </span>;
}