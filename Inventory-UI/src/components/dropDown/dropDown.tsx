import React from "react";

export default function DropDown({title, children, index}: {title: React.ReactNode, children: React.ReactNode[], index: number}) {

    return(<>
        <button className="btn" popoverTarget={`popover-${index}`} style={{ anchorName: `--anchor-${index}` } as React.CSSProperties }>
            {title}
        </button>
        <ul className="dropdown menu w-52 rounded-box bg-base-100 shadow-sm"
            popover="auto" id={`popover-${index}`} style={{ positionAnchor: `--anchor-${index}` } as React.CSSProperties }>
            { children.map((child, index) => {
                return <li key={index}>{child}</li>
            })}
        </ul>
    </>)
}