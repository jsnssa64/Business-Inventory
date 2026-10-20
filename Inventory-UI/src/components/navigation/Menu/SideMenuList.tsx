import React, { useState } from "react";
import MenuSpan from "../../Navigation/Menu/MenuSpan";
import MenuDropdown from "../../Navigation/Menu/MenuDropdown";
import { MenuLink } from "./Abstract/menuLink";

export default function SideMenuList ({ parentTitle, childTitles }: { parentTitle:string, childTitles:MenuLink[]}) {
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