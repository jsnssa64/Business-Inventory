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

    return (
    <div className="flex items-center"> 
      <div className="join-left">
        showing {minItemsCount}-{maxItemCount} of {totalItemCount} items
      </div>
      <div className="join-right">
          <button {...pageAction(false)}>Previous</button>
          <button {...pageAction(false, true)}>P</button>
          <button key="leftdots" aria-label="Previous pages" {...setActivePage(startingIndexOutOfBounds ? 0 : rangeStartIndex - 1, startingIndexOutOfBounds)}>...</button>
          {Array.from(
            { length: clampedPagingRangeCount }, 
            (_, pagingIndex) => {
              const pageIndexInRange = rangeStartIndex + pagingIndex;
              const pageNumber = pageIndexInRange + 1;
              return  (<button key={pagingIndex} aria-label={`Page ${pageNumber}`} {...setActivePage(pageIndexInRange, pageIndexInRange == currentPageIndex)} >{pageNumber}</button>)
          })}
          <button key="rightdots" aria-label="More pages" {...setActivePage(endingIndexOutOfBounds ? clampedTotalPageCount - 1 : rangeEndIndex, endingIndexOutOfBounds)}>...</button>
          <button {...pageAction(true)}>Next</button>
          <button {...pageAction(true, true)}>N</button>
      </div>
    </div>)
}
