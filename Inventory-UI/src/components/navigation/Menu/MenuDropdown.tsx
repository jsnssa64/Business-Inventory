import React, { useState } from "react";
import { Link } from "react-router-dom";
import { MenuLink } from "./Abstract/menuLink";


export default function MenuDropdown ({ values, isVisible, children }: { values: MenuLink[], isVisible:boolean, children?: React.ReactNode }) {

    return <ul className={`menu-dropdown p-0 ${isVisible ? 'menu-dropdown-show' : ''}`}>
                    {values.map((value, index) => (
                        <li key={index} className="menu-title border-slate-900 !p-0 hover:bg-slate-100">
                            <Link to={value.LinkTo} className="p-2">{value.Title}</Link>
                        </li>
                    ))}
                    {children}
                </ul>
}