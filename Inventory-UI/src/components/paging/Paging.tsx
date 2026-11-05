import React, { useState } from "react";

interface PagingBarProps {
    totalItemCount: number,
    totalPageCount: number,
    pagingRangeCount: number,
    currentPageIndex: number,
    setPageIndex: React.Dispatch<React.SetStateAction<number>>,
    itemsPerPage: number
}

type ButtonBaseType = React.ButtonHTMLAttributes<HTMLButtonElement>;

const PAGE_BUTTON_CLASS = "btn join-item btn-outline";

export default function Paging({ totalItemCount, totalPageCount, pagingRangeCount, currentPageIndex, setPageIndex, itemsPerPage}: PagingBarProps) {
    
    //  Minimum of 1 page in the range, and minimum of 0 pages in total
    const clampedPagingRangeCount = Math.max(Math.floor(pagingRangeCount), 1);
    const clampedTotalPageCount = Math.max(Math.floor(totalPageCount), 0);

    //  Calculate where the current page is within the range of pages to display
    const pageOffsetWithinRange: number = currentPageIndex % clampedPagingRangeCount;

    //  Calculate the starting and ending indices of the range of pages to display
    const rangeStartIndex: number = currentPageIndex - pageOffsetWithinRange;
    const rangeEndIndex: number = rangeStartIndex + clampedPagingRangeCount;

    //  Calculate the minimum and maximum item counts for the current page
    const minItemsCount = (currentPageIndex * itemsPerPage) + 1;
    const maxItemCount = Math.min((currentPageIndex + 1) * itemsPerPage, totalItemCount);

    //  Determine if the starting or ending indices are out of bounds
    const endingIndexOutOfBounds = rangeEndIndex >= clampedTotalPageCount;
    const startingIndexOutOfBounds = rangeStartIndex <= 0;

    const incrementPage= () => setPageIndex((prev) => Math.min(prev + 1, clampedTotalPageCount));
    const decrementPage= () => setPageIndex((prev) => Math.max(prev - 1, 0));

    const pageAction = (isNext: boolean, useSmall: boolean = false): ButtonBaseType => {
      const label: string = isNext ? "Next page" : "Previous page";
      const isEndOfPages: boolean = (currentPageIndex >= clampedTotalPageCount - 1);
      const isStartOfPages: boolean = (currentPageIndex == 0);
      const disable: boolean = isNext ? isEndOfPages : isStartOfPages;

      return {
        'aria-label': label,
        className: `${isNext ? 'rounded-r-md' : 'rounded-l-md'} ${PAGE_BUTTON_CLASS} ${useSmall ? "block md:hidden" : "hidden md:inline"}`,
        disabled: disable,
        onClick: () => {
            isNext ? incrementPage() : decrementPage();
        }
      }
    }

    const setActivePage = (index: number, isActive: boolean): ButtonBaseType =>  {
      const isCurrentPage = index === currentPageIndex;
      return {
          className: `${PAGE_BUTTON_CLASS} ${isActive ? 'btn-active' : ''}`,
          'aria-current': isCurrentPage ? 'page' : undefined,
          onClick: () => {
            setPageIndex(index);
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
