import './style/index.css';
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import ProfileWrapper from "./components/profile/ProfileWrapper";
import Inventory from "./components/Inventory/Inventory";

const queryClient = new QueryClient()

export default function App() {
  return <QueryClientProvider client={queryClient}>
            <ReactQueryDevtools initialIsOpen={false} />
            <BrowserRouter>
              <Routes>
                <Route path="/Inventory" element={<ProfileWrapper/>}>
                  <Route path=":userId" element={<Inventory/>} />
                  <Route path="User" element={<Inventory/>} />
                </Route>
              </Routes>
            </BrowserRouter>
          </QueryClientProvider>
}
