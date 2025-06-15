import React from "react";
import { Link } from "react-router-dom";


export default function ProfileNavBar() {
    return (<>
        <input type="text" placeholder="Search" className="input input-bordered w-24 md:w-auto" />
        <div className="dropdown dropdown-end">
            <div tabIndex={0} role="button" className="btn btn-ghost btn-circle avatar">
                <div className="w-10 rounded-full">
                    <img
                        alt="Navbar component"
                        src="https://img.daisyui.com/images/stock/photo-1534528741775-53994a69daeb.webp" />
                </div>
            </div>
            <ul
                tabIndex={0}
                className="menu menu-sm dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow">
                <li>
                    <Link className="justify-between" to={"/profile"}>
                        Profile
                    <span className="badge">New</span>
                    </Link>
                </li>
                <li><Link  className="justify-between" to={"/settings"}>Settings</Link></li>
                <li><Link  className="justify-between" to={"/Logout"}>Logout</Link></li>
            </ul>
        </div></>)
}