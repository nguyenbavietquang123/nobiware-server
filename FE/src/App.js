import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import ScopeConsent from "./ScopeConsent/ScopeConsent";
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/consent" element={<ScopeConsent />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
