import { Route, Routes } from "react-router-dom"
import Login from "./components/Login"
import Home from "./components/Home"
import ChangePassword from "./components/ChangePassword"
import AddPatient from "./components/AddPatient"
import EditPatient from "./components/EditPatient"
import './App.css';
import { Fragment } from "react"
import 'react-toastify/dist/ReactToastify.css'
import NavMenu from "./components/NavMenu/NavMenu"

function App(props) {
  return (
    <Fragment>
      <NavMenu/>
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
