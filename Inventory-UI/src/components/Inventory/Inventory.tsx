import React, { useEffect, useRef } from "react";
import { useState } from "react";
import PagingBar from "../paging/Paging";
import DataTable from "../dataTable/DataTable";
import { Level } from "../../models/ui/user/permission";
import Tile from "../tiles/tile";

export default function Inventory({ maxPages = 10, maxAvailablePages = 2 }: { maxPages?: number, maxAvailablePages?:number, currentPage?: number }) {
    const [currentPage, setCurrentPage] = useState(0);
    const [permissionLevel, setPermissionLevel] = useState(Level.read_only);

    useEffect(() => {
      const setUserPermission = async () => {
        // await userService.GetUser();
        const user: { Role: string } = { Role: "write_delete" }; 
        
        const userRole = user.Role as unknown as Level;
        setPermissionLevel(Level[userRole]);
      };
      setUserPermission();
    }, []);
    

    
    return (
      <div className="overflow-x-auto w-full p-4">
        <h1 className="text-2xl font-bold mb-4">Inventory</h1>
        <div className="grid grid-cols-4">
          <div className="grid grid-cols-1 gap-4">
            <Tile title={""} description={""} footer={""} imageUrl={""}>
              <div>
                TEst
              </div>
            </Tile>
          </div>
          <div className="grid grid-cols-1 gap-4">
            <Tile title={""} description={""} footer={""} imageUrl={""}>
              <div className="grid grid-cols-1 gap-4">
                <div className="">Test</div>
              </div>
            </Tile>
          </div>
          
        </div>
        <div className="flex flex-col justify-center items-center">
          <DataTable permission={permissionLevel} data={[{id:'1', 'test': 'test', 'another' : 'testanother'}, {id: '2', 'test': 'test2', 'another' : 'teswtanother'}]} />
          <PagingBar
            totalPages={maxPages}
            maxAvailablePages={maxAvailablePages}
            currentPage={currentPage}
            setCurrentPage={setCurrentPage}
            incrementPage={() => setCurrentPage((p) => Math.min(p + 1, maxPages))}
            deIncrementPage={() => setCurrentPage((p) => Math.max(p - 1, 0))}
          />
        </div>
      </div>
    );
  }