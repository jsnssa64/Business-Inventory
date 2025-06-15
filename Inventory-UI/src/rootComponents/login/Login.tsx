import React, { useState } from "react";

export default function App() {
  const [isVisible, setIsVisible] = useState(true);

  const SetSideMenuVisible = () => {
      setIsVisible(!isVisible);
  };

  return (
    <>
      <div className="container mx-auto px-4">
      </div>
    </>
  );
}
