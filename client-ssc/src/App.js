import { Route, Routes } from "react-router-dom"
import Login from "./components/Login"
import Home from "./components/Home"
import ChangePassword from "./components/ChangePassword"
import AddPatient from "./components/AddPatient"
import EditPatient from "./components/EditPatient"
import './App.css';
import { Fragment, useEffect, useState } from "react"
import jwt_decode from "jwt-decode"
import jwt from "jwt-decode"
import 'react-toastify/dist/ReactToastify.css'
import Unauthorized from "./components/Unauthorized"

function App(props) {

  const [role, setRole] = useState("")

  useEffect(() => {
    var excetpions = ["/login", "/changePassword", "/"];
    if (excetpions.indexOf(window.location.pathname) > -1) {
      return;
    }

    const tokenRead = localStorage.getItem("token");

    if (tokenRead == null || tokenRead === "") {
      window.location = "/login"
    }
    else {
      var dateNow = new Date();
      var decodedToken = jwt_decode(tokenRead, { complete: true });
      if (decodedToken.exp >= dateNow.getTime()) {
        localStorage.removeItem("token");
        window.location = "/login"
      }
      const decoded = jwt(tokenRead);
      setRole(decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"])
    }
  }, []);

  return (
    <Fragment>
      <Routes>
        <Route path="/login" exact element={<Login />} />
        <Route path="/changePassword" exact element={<ChangePassword />} />
        <Route path="/" exact element={<Home />} />
        <Route path="/addPatient" exact element={<AddPatient />} />
        <Route path="/editPatient" exact element={<EditPatient />} />
      </Routes>
      {/*<ToastContainer />*/}
    </Fragment>
  );
}

export default App;
