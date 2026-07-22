import React, { useState } from "react";
import MenuDropdown from "./MenuDropdown";
import MenuSpan from "./MenuSpan";
import { MenuLinkType } from "./Abstract/MenuLinkType";

export default function SideMenuList ({ parentTitle, childTitles }: { parentTitle:string, childTitles:MenuLinkType[]}) {
    const [isVisible, setIsVisible] = useState<boolean>(false);

    const handleClick = () => {
        setIsVisible(!isVisible);
    }

    return <>
        <MenuSpan title={parentTitle} isVisible={isVisible} onClick={handleClick}>{parentTitle}</MenuSpan>
        <MenuDropdown values={childTitles} isVisible={isVisible}>
        </MenuDropdown>
    </>
}