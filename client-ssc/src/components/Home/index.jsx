import React, { useState, useEffect } from "react";
import request from "../Request";
import jwt from "jwt-decode"

export default function Home() {
    const [userData, setUserData] = useState([])
    const [addedPatients, setAddedPatients] = useState([])

    useEffect(() => {
        const handleChange = async () => {
            const tokenRead = localStorage.getItem("token");
            if (tokenRead == null || tokenRead === '') {
                window.location = '/login'
            }
            const decoded = jwt(tokenRead);
            const urlUser = '/api/User/userDetails/' + decoded["nameid"];
            console.log(urlUser)
            const callbackUser = (response) => {
                var newDataArr = Object.keys(response.data).map((key) => response.data[key]);
                setUserData(newDataArr)
            }
            await request({ url: urlUser, type: "GET" }, callbackUser);

            const urlAddedPatients = "/api/Patient/recentlyAddedPatients";
            const callbackurlAddedPatients = (response) => {
                var newDataArr = Object.keys(response.data).map((key) => response.data[key]);
                setAddedPatients(newDataArr)
            }
            await request({ url: urlAddedPatients, type: "GET" }, callbackurlAddedPatients);
        }
        handleChange()
    }, []);

    return (<div>
        <div style={{ height: "50px" }}></div>
        <div className="product-container">
            Tymczasowe wyświetlanie:
            <br></br>
            Twoje dane: {userData.join(", ")}
            <br></br>
            Ostatnio dodani pacjenci: {addedPatients.join(", ")}
        </div>
    </div>
    );
};