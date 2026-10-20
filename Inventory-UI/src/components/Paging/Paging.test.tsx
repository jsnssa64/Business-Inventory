import {render, screen} from '@testing-library/react';
import { userEvent } from '@testing-library/user-event';
import Paging from './Paging';
import React from 'react';

test('renders the correct item-count label', () =>  {
    let itemsPerPage: number = 10;
    let totalItemCount: number = 100;
    render(<Paging 
        currentPageIndex = {0}
        totalPageCount={100}
        pagingRangeCount = {5}
        totalItemCount = {totalItemCount}
        itemsPerPage = {itemsPerPage}
        setPageIndex = {jest.fn()}
    />);

    expect(screen.getByText(`showing 1-${itemsPerPage} of ${totalItemCount} items`)).toBeInTheDocument();
});


test('onClick of next page', async() => {
    const setPageIndexMock = jest.fn();
    
    render(<Paging 
        currentPageIndex = {0}
        totalPageCount={100}
        pagingRangeCount = {5}
        totalItemCount = {100}
        itemsPerPage = {10}
        setPageIndex = {setPageIndexMock}
        />)

    const nextButton = screen.getAllByRole('button', {name: 'Next page'});

    await userEvent.click(nextButton[0]);

    expect(setPageIndexMock).toHaveBeenCalledTimes(1);

    //  [0] - Get Function Arrays
    //  [0] - Get First Call
    //  [0] or [1] - get first or second input e.g func(a,b) [0] = a, [1] = b
    let func = setPageIndexMock.mock.calls[0][0];

    expect(setPageIndexMock).toHaveBeenCalledWith(1);
})