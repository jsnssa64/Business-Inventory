import React, { useEffect, useState } from "react";

interface PagingBarProps {
    totalPages: number,
    maxAvailablePages: number,
    currentPage: number,
    incrementPage: () => void,
    deIncrementPage: () => void,
    setCurrentPage: (page: number) => void
}

export default function Paging({ totalPages, maxAvailablePages, currentPage, incrementPage, deIncrementPage, setCurrentPage }: PagingBarProps) {
    const currentPageModulo: number = currentPage % maxAvailablePages;
    const startingIndex: number = currentPage - currentPageModulo;
    const endingIndex: number = startingIndex + maxAvailablePages;


    const previousPage = (useSmall: boolean = false) => {
      let isStartOfPages: boolean = (currentPage == 0);
      return {
        className: `rounded-l-md btn join-item btn-outline ${useSmall ? "block md:hidden" : "hidden md:inline"}`,
        disabled: isStartOfPages,
        style: (isStartOfPages) ? { backgroundColor: "silver", color: "white", cursor: "not-allowed" }
        : {},
        onClick: () => {
            deIncrementPage();
        }
      }
    }

    const nextPage = (useSmall: boolean = false): any => {
      let isEndOfPages: boolean = (currentPage == totalPages - 1);
      return {
        className: `rounded-r-md btn join-item btn-outline ${useSmall ? "block md:hidden" : "hidden md:inline"}`,
        disabled: isEndOfPages,
        style: (isEndOfPages) ? 
          { backgroundColor: "silver", color: "white", cursor: "not-allowed" }
          : {},
        onClick: () => {
            incrementPage();
        }
      }
    }

    const setActivePage = (isActive: boolean): any => {
    {
      return {
        className: `btn join-item btn-outline ${isActive ? 'btn-active' : ''}`        }
      } 
    }


    const setActiveIndex = (index: number): any => {
        return {
          onClick: (e: React.MouseEvent<HTMLButtonElement>) => {
            setCurrentPage(index);
          }
        }    
      }

        return (
        <div className="flex items-center"> 
          <div className="join-left">
            showing 1-200 of {totalPages * 200} items
          </div>
          <div className="join-right">
              <button {...previousPage()}>Previous</button>
              <button {...previousPage(true)}>P</button>
              {startingIndex > 0 && <button key="leftdots" {...setActivePage(false)} {...setActiveIndex(startingIndex - 1)}>...</button>}
              {Array.from({length: maxAvailablePages }, (_, index) => {
                  var pageIndexItem = startingIndex + index;

                  return  (<button key={index} {...setActivePage(pageIndexItem == currentPage)} {...setActiveIndex(pageIndexItem)} >{pageIndexItem + 1}</button>)
              })}
              {endingIndex < totalPages && <button key="rightdots" {...setActivePage(false)} {...setActiveIndex(endingIndex)}>...</button>}
              <button {...nextPage()}>Next</button>
              <button {...nextPage(true)}>N</button>
          </div>
        </div>)
}