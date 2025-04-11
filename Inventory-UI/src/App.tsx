import React from "react";
import InventoryTable from "./components/inventoryTable/inventoryTable";
import Navigation from "./components/navigation/navigation";

export default function App() {
  return (
    <>
      <div className="container mx-auto px-4">
        <Navigation />
        <h1>Hello, world!</h1>
        <InventoryTable />
      </div>
    </>
  );
}
