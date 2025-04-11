import React, { useEffect, useRef } from "react";
import { useState } from "react";
import * as Inventory from "../../hooks/useInventory";
import { InventoryItem, InventoryItemKeys } from "../../models/data/InventoryItem";

export default function inventoryTable({ itemsPerPage = 1, maxAmountPagesVisible = 2 }: { itemsPerPage?: number, maxAmountPagesVisible?: number }) {
    
    const [currentSelectedPageIndex, setCurrentSelectedPageIndex] = useState(0);
    const [totalPages, setTotalPages] = useState(0);

    const [subSetMaxIndex, setSubSetMaxIndex] = useState(0);
    const [subSetCurrentIndex, setSubSetCurrentIndex] = useState(0);
    const paginationRef = useRef<(HTMLButtonElement | null)[]>([]);

    var myArray: Array<InventoryItem> = [];
    myArray.push({
        id: "2",
        name: "item2",
        description: "item2 description",
        quantity: 11,
        price: 120,
    });
    myArray.push({ 
      id: "1", 
      name: "item1", 
      description: "item1 description", 
      quantity: 10, 
      price: 100 
    });
    myArray.push({
      id: "3",
      name: "item3",
      description: "item3 description",
      quantity: 15,
      price: 150,
    });

    myArray.push({
      id: "3",
      name: "item3",
      description: "item3 description",
      quantity: 15,
      price: 150,
    });

    useEffect(() => { 
      setTotalPages(Math.ceil(myArray.length / itemsPerPage));
    }, []);

    useEffect(() => { 
      setSubSetMaxIndex(totalPages - (maxAmountPagesVisible-1));
    }, [totalPages]);

    const setPaginationProp = (index: number): any => {
      return {
        className: `join-item btn btn-outline ${index == currentSelectedPageIndex ? 'btn-active' : ''}`,
        onClick: (e: React.MouseEvent<HTMLButtonElement>) => {
          //  Refresh Table
          // Inventory.useAllInventory();
          e.preventDefault();
          clearActive();
          e.currentTarget.classList.add("btn-active");
          setCurrentSelectedPageIndex(index);
        }
      }    
    }

    const clearActive = () => {
      paginationRef.current.forEach((button) => {
        if (button) {
          button.classList.remove("btn-active");
        }
      });
    }

    const IsPrevArrowVisible = () => {
      return subSetCurrentIndex <= subSetMaxIndex && subSetCurrentIndex > 0;
    }

    const IsNextArrowVisible = () => { 
      return subSetCurrentIndex >= 0 && subSetCurrentIndex < subSetMaxIndex;
    }

    const next = (): any => {
      return {
        className: "join-item btn btn-outline",
        onClick: () => {
          if (pageIndex == totalPages - 1) return;
          // setPageIndex(pageIndex + 1);
          if (subSetCurrentIndex < subSetMaxIndex) {
            setSubSetCurrentIndex(subSetCurrentIndex + 1);
            paginationRef.current[subSetCurrentIndex + 1]?.focus();
          } else {
            setSubSetCurrentIndex(0);
            paginationRef.current[0]?.focus();
          }
        }
      }
    }

    const previous = () => {
      return {
        className: "join-item btn btn-outline",
        onClick: () => {
          // if (pageIndex == 1) return;

          // setPageIndex(pageIndex - 1);
        }
      }
    }
    // const { status, data, error } = Inventory.useAllInventory();
    // console.log("data", data);
    // console.log("status", status);  
    // console.log("error", error);
  
    // if (status === 'pending') {
    //   return <span>Loading...</span>
    // }
  
    // if (status === 'error') {
    //   return <span>Error: {error.message}</span>
    // }
  
    // useEffect(() => {
    //   Inventory.useAllInventory();
    // }, []);

    return (
      <div className="overflow-x-auto">
        <table className="table">
          <thead>
            <tr>{ InventoryItemKeys.map((key) => <th key={key}>{key}</th>) }</tr>            
          </thead>
          <tbody>
            { myArray.map((item, itemIndex) =>  {
              if (itemIndex < currentSelectedPageIndex * itemsPerPage || itemIndex >= (currentSelectedPageIndex + 1) * itemsPerPage) return null;
              return (
                <tr key={itemIndex}>
                  <td>{item.name}</td>
                  <td>{item.description}</td>
                  <td>{item.price}</td>
                  <td>{item.quantity}</td>
                </tr>
              )
            })}
          </tbody>
        </table>
        <div className="join flex flex-row">
          {IsPrevArrowVisible()  && <button {...previous()}>Previous</button>}
          {Array.from({length: totalPages}, (_, index) => {
            if(index < subSetCurrentIndex) return null;
            if(index  >= subSetCurrentIndex + maxAmountPagesVisible) return null;

            return  (<button key={index} ref={(button) => (paginationRef.current[index] = button)} {...setPaginationProp(index)} >{index}</button>)
          })}
          {IsNextArrowVisible() && <button {...next()}>Next</button>}
        </div>
      </div>
    );
  }