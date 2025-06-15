import React from "react";
import userService from "../../api/services/userService";
import { Level } from "../../models/ui/user/permission";
import { Link } from "react-router-dom";
import Icon from "../svg/icon";
import DropDown from "../dropDown/dropDown";

type identifierObject = { id: string | number }

type ObjectViewerProps<T extends identifierObject> = {
    data: T[];
    permission: Level;
    actions?: {
        onEdit?: (item: T) => void;
        onDelete?: (item: T) => void;
        onView?: (item: T) => void;
        onCreate?: () => void;
    }
};


export default function DataTable<T extends identifierObject>({ data, permission, actions }: ObjectViewerProps<T>) {
    if (!data || data.length === 0) return <p>No data available</p>;

    const keys = Object.keys(data[0]) as (keyof T)[];

    const editControls = (item: T, permission : Level): React.ReactNode[] => {
        switch(permission) {
            case Level.write_delete:
                return ([
                        <Link to={`/View/${item.id}`} onClick={() => actions?.onView?.(item)}>View</Link>,
                        <Link to={`/Edit/${item.id}`} onClick={() => actions?.onEdit?.(item)}>Edit</Link>,
                        <Link to={`/Delete/${item.id}`} onClick={() => actions?.onDelete?.(item)}>Delete</Link>
                    ]);
            case Level.write:
                return ([
                        <Link to={`/View/${item.id}`} onClick={() => actions?.onView?.(item)}>View</Link>,
                        <Link to={`/Edit/${item.id}`} onClick={() => actions?.onEdit?.(item)}>Edit</Link>
                    ])
            case Level.read_only:
                return ([
                        <button onClick={() => actions?.onView?.(item)}>View</button>
                    ]);
            default:
                return [];
        }            
    }

    return (
        <>
            <table className="table">
                <thead>
                    <tr>
                        {keys.map((key) => (
                            <th key={String(key)}>{String(key)}</th>
                        ))}
                        <th>Edit</th>
                    </tr>            
                </thead>
                <tbody>
                {data.map((item, i) => (
                    <tr key={i}>
                        {Object.values(item).map((value, j) => (
                            <td key={j}>{String(value)}</td>
                        ))}
                        <td>
                            <DropDown title={<><Icon></Icon></>} index={i} children={editControls(item, permission)}></DropDown>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>           
        </>
    )

}