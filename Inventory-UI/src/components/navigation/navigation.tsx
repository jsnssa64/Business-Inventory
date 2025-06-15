import React, { useState } from "react";
import RightNavBar from "../ProfileNavBar/ProfileNavBar";

interface NavigationProps {
    setSideMenuVisible: () => void;
}

export default function Navigation({ setSideMenuVisible } : NavigationProps) {
    

    return (
        <>
            <div className="navbar bg-base-100 shadow-sm">
                <div className="flex-none">
                    <button onClick={setSideMenuVisible} className="btn btn-square btn-ghost">
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block h-5 w-5 stroke-current"> 
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h16"></path> 
                        </svg>
                    </button>
                </div>
                <div className="flex-1">
                    <a className="btn btn-ghost text-xl">Inventory</a>
                </div>
                <div className="flex gap-2">
                    <RightNavBar></RightNavBar>
                </div>
            </div>       
        </>
    )
}