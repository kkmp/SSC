import React, { useEffect, useState } from "react";
import getTokenData from "../GetTokenData";

const NavMenu = () => {
    const [show, setShow] = useState(true)
    const [role, setRole] = useState("")
    const [name, setName] = useState("")
    const [surname, setSurname] = useState("")

    useEffect(() => {
        var excetpions = ["/login"]; //change password i inne
        if (excetpions.indexOf(window.location.pathname) > -1) {
            setShow(false)
            return;
        }
        const data = getTokenData()
        if (data != null) {
            setName(data.name)
            setSurname(data.surname)
            setRole(data.role)
        }
    }, [])

    return (
        show ? <div>
            MENU XD
            <br></br>
            {name}<br></br>{surname}<br></br>{role}<br></br>
            Ale przynajmniej jest i działa jak trzeba XD
        </div> : <div></div>
    );
}

export default NavMenu