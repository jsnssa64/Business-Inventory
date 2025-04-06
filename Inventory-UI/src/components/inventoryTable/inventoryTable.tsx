import React from "react";
import { useState } from "react";


export default function Counter() {
    const [count, setCount] = useState(0);
    return (
      <div>
          <button className="btn w-64 rounded-full" onClick={() => setCount(count + 1)}>
          <span className="text-2xl font-bold">{count}</span>
          </button>
      </div>
    );
  }