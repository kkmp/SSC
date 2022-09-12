import React, { useState } from "react";
import Error from "../Error";
import './signin.css'
import request from "../Request";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const initialValue = [];
    const [error, setError] = useState(initialValue);

    const handleSubmit = async e => {
        e.preventDefault();
        const url = '/api/Login/login'
        const data = {
            email, password
        }
        const callback = (response) => {
            localStorage.setItem('token', response.data["token"])
            window.location = '/'
        }
        const errorCallback = (response) => {
            var newErrorArr = Object.keys(response.data.errors).map((key) => response.data.errors[key].join(" "));
            setError(newErrorArr)
        }
        await request({url: url, data: data, type: "POST"}, callback, errorCallback, false);
    };
    
    return (
        <div className="container pt-5 mt-5">
            <div className="row">
                <section className="login-logo mb-5 text-center">
                    <h1>Logowanie</h1>
                </section>
                <div>
                    {error.map((err, idx) => <Error message={err} key={idx} />)}
                </div>
                <div className="col-4 offset-4">
                    <form onSubmit={handleSubmit}>
                        <div className="form-group pb-2">
                            {/*Adres email*/}
                            <input type="email" name="email" className="form-control" placeholder="Email" value={email} onChange={({ target }) => setEmail(target.value)} required/>
                        </div>
                        <div className="form-group pb-2">
                            {/*Hasło*/}
                            <input type="password" name="password" className="form-control" placeholder="Hasło" value={password} onChange={({ target }) => setPassword(target.value)} required />
                        </div>
                        <button type="submit" id="signin" className="btn btn-primary w-100 my-3 py-2">Zaloguj się</button>
                    </form>
                </div>
                <div className="text-center mt-1">
                    <p>Nie pamiętam hasła <a className="text-decoration-none link-success fw-bold" href="register">Zmień hasło!</a></p>
                </div>
            </div>
        </div>
    );
};
