import React, { useState } from "react";
import Navigation from "../../components/Navigation/Navigation";
import LeftMenu from "../../components/Navigation/Menu/LeftMenu";
import { Outlet } from "react-router-dom";

export default function App() {
  const [isVisible, setIsVisible] = useState(true);

  const SetSideMenuVisible = () => {
      setIsVisible(!isVisible);
  };

  return (
    <>
      <div className="container mx-auto px-4">
        <LeftMenu title="Incoming Test" isMenuVisible={isVisible}></LeftMenu>
        <Navigation setSideMenuVisible={SetSideMenuVisible}></Navigation>
        <div className="flex flex-row">
          <Outlet></Outlet>
        </div>
      </div>
    </>
  );
}
