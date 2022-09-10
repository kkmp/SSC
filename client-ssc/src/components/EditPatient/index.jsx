import React, { Fragment, useState, useEffect } from "react";
import { toast } from 'react-toastify';
import Error from "../Error";
import putRequest from "../RequestPut";
import getRequest from "../RequestGet";
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

    const id = "08da9356-32b5-4682-84f1-cbbbf83c2435" //TYMCZASOWO!!!
    const [pesel, setPesel] = useState("");
    const [name, setName] = useState("");
    const [surname, setSurname] = useState("");
    const [sex, setSex] = useState("");
    const [birthDate, setBirthDate] = useState("");
    const [street, setStreet] = useState("");
    const [address, setAddress] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [cityName, setCityName] = useState("");
    const [citizenshipName, setCitizenshipName] = useState("");
    const initialValue = [];
    const [error, setError] = useState(initialValue);

    useEffect(() => {
        const handleChange = async () => {
            const url = 'https://localhost:7090/api/Patient/patientDetails/' + id
            const callback = (response) => {
                setPesel(response.data.pesel)
                setName(response.data.name)
                setSurname(response.data.surname)
                setSex(response.data.sex)
                setBirthDate(response.data.birthDate.split(" ")[0])
                setStreet(response.data.street)
                setAddress(response.data.address)
                setPhoneNumber(response.data.phoneNumber)
                setCityName({ value: response.data.city, label: response.data.city })
                setCitizenshipName({ value: response.data.citizenship, label: response.data.citizenship })
            }
            await getRequest(url, callback);
        }
        handleChange()
    }, []);

    const handleSubmit = async e => {
        e.preventDefault();
        const url = 'https://localhost:7090/api/Patient/editPatient'
        const data = {
            "id": id,
            "name": name,
            "surname": surname,
            "street": street,
            "address": address,
            "phoneNumber": phoneNumber,
            "cityName": cityName.value,
            "citizenshipName": citizenshipName.value,
        }
        const callback = () => {
            toast.success("Zapisano zmiany!", { position: toast.POSITION.BOTTOM_RIGHT });
            setError([])
        }
        const errorCallback = (response) => {
            var newErrorArr = Object.keys(response.data.errors).map((key) => response.data.errors[key].join(" "));
            setError(newErrorArr)
        }
        await putRequest(url, data, callback, errorCallback);
    };

    return (<Fragment>
        <div>
            {error.map((err, idx) => <Error message={err} key={idx} />)}
        </div>
        <form onSubmit={handleSubmit} className="mt-5">
            <h3>Tymczasowe edytuj pacjenta</h3>
            <input type="text" name="pesel" readOnly value={pesel} onChange={({ target }) => setPesel(target.value)} required />
            <input type="text" name="name" value={name} onChange={({ target }) => setName(target.value)} required />
            <input type="text" name="surname" value={surname} onChange={({ target }) => setSurname(target.value)} required />
            <div>
                <input value="F" type="radio"  name="sex" id="female" readOnly checked={sex === 'F'}  />Kobieta
                <input value="M" type="radio" name="sex" id="male" readOnly checked={sex === 'M'}  />Mężczyzna
            </div>
            <input type="text" name="birthDate" readOnly value={birthDate} onChange={({ target }) => setBirthDate(target.value)} required />
            <input type="text" name="street" value={street} onChange={({ target }) => setStreet(target.value)} required />
            <input type="text" name="address" value={address} onChange={({ target }) => setAddress(target.value)} required />
            <input type="text" name="phoneNumber" value={phoneNumber} onChange={({ target }) => setPhoneNumber(target.value)} required />
            <Select required
                value={cityName}
                onChange={setCityName}
                options={citiesOptions}
            />
            <Select required
                value={citizenshipName}
                onChange={setCitizenshipName}
                options={citizenshipsOptions}
            />
            <button type="submit" className="btn btn-primary btn-lg w-100">Zapisz zmiany</button>
        </form>
    </Fragment>
    );
}

export default AddProduct