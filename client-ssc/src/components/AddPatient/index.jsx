import React, { Fragment, useState } from "react";
import { toast } from 'react-toastify';
import Error from "../Error";
import request from "../Request";
import Select from 'react-select';

const AddProduct = () => {
    const citiesOptions = [ //to będzie trzeba z bazy pobrać kiedyś
        { value: 'Warszawa', label: "Warszawa" },
        { value: 'Lublin', label: "Lublin" },
    ];

    const citizenshipsOptions = [ //tak samo
        { value: 'Obywatelstwo polskie', label: "Obywatelstwo polskie" },
        { value: 'Obywatelstwo francuskie', label: "Obywatelstwo francuskie" },
    ];

    const [pesel, setPesel] = useState("");
    const [name, setName] = useState("");
    const [surname, setSurname] = useState("");
    const [sex, setSex] = useState("F");
    const [birthDate, setBirthDate] = useState("");
    const [street, setStreet] = useState("");
    const [address, setAddress] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [cityName, setCityName] = useState("");
    const [citizenshipName, setCitizenshipName] = useState("");
    const initialValue = [];
    const [error, setError] = useState(initialValue);

    const handleSubmit = async e => {
        e.preventDefault();
        const url = '/api/PAtient/addPatient'
        const data = {
            "pesel": pesel,
            "name": name,
            "surname": surname,
            "sex": sex,
            "birthDate": birthDate,
            "street": street,
            "address": address,
            "phoneNumber": phoneNumber,
            "cityName": cityName.value,
            "citizenshipName": citizenshipName.value,
        }
        const callback = () => {
            toast.success("Pacjent został dodany!", { position: toast.POSITION.BOTTOM_RIGHT });
            setError([])
        }
        const errorCallback = (response) => {
            var newErrorArr = Object.keys(response.data.errors).map((key) => response.data.errors[key].join(" "));
            setError(newErrorArr)
        }
        await request({url: url, data: data, type: "POST"}, callback, errorCallback);
    }

    const handleChange = (event) => {
        setSex(event.target.value)
    }

    return (<Fragment>
        <div>
            {error.map((err, idx) => <Error message={err} key={idx} />)}
        </div>
        <form onSubmit={handleSubmit} className="mt-5">
            <h3>Tymczasowe dodaj pacjenta</h3>
            <input type="text" name="pesel" value={pesel} onChange={({ target }) => setPesel(target.value)} required />
            <input type="text" name="name" value={name} onChange={({ target }) => setName(target.value)} required />
            <input type="text" name="surname" value={surname} onChange={({ target }) => setSurname(target.value)} required />
            <div>
                <input value="F" type="radio" name="sex" id="female" checked={sex === 'F'} onChange={handleChange} />Kobieta
                <input value="M" type="radio" name="sex" id="male" checked={sex === 'M'} onChange={handleChange} />Mężczyzna
            </div>
            <input type="date" name="birthDate" value={birthDate} onChange={({ target }) => setBirthDate(target.value)} required />
            <input type="text" name="street" value={street} onChange={({ target }) => setStreet(target.value)} required />
            <input type="text" name="address" value={address} onChange={({ target }) => setAddress(target.value)} required />
            <input type="text" name="phoneNumber" value={phoneNumber} onChange={({ target }) => setPhoneNumber(target.value)} required />
            <Select required
                onChange={setCityName}
                options={citiesOptions}
            />
            <Select required
                onChange={setCitizenshipName}
                options={citizenshipsOptions}
            />
            <button type="submit" className="btn btn-primary btn-lg w-100">Dodaj pacjenta</button>
        </form>
    </Fragment>
    );
}

export default AddProduct