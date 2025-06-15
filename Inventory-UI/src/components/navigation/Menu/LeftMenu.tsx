import React, { useState } from "react";
import SideMenuList from "./SideMenuList";
import { Link } from "react-router-dom";

type LeftMenu = {
    title: string,
    isMenuVisible: boolean
}


export default function LeftMenu({ title, isMenuVisible }: LeftMenu) {
    
    return ( 
        <> 
            <aside className={`transition-all duration-300 ${isMenuVisible ? "w-64" : "w-0 overflow-hidden"}`}>
                <div className="h-full w-full bg-base-200 p-4">
                    <ul className="menu bg-base-200 rounded-box w-full p-2">
                        <li className="menu-title hover:bg-slate-700 border-slate-900">
                            <Link to={"/Inventory"}>{title}</Link>
                        </li>
                        <li>
                            <SideMenuList parentTitle="Test" childTitles={[{ LinkTo: "/Item1", Title: "Item"}, { LinkTo: "/Item2", Title: "Item2"}]}>
                            </SideMenuList>
                        </li>
                        <li className="menu-title">
                            <Link to={"/Test"} ></Link>
                        </li>
                    </ul>
                </div>
            </aside>           
        </>
    )
}