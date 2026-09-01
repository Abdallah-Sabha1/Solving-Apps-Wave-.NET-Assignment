import { StrictMode, useState } from "react";
import { createRoot } from "react-dom/client";
import "./styles.css";

const API = "https://localhost:7000/api/bookings";

function App() {
  const [resourceId, setResourceId] = useState("room-1");
  const [userId, setUserId] = useState("user-1");
  const [start, setStart] = useState("2026-09-01T10:00");
  const [end, setEnd] = useState("2026-09-01T11:00");
  const [bookings, setBookings] = useState([]);
  const [message, setMessage] = useState("");

  async function loadBookings() {
    const response = await fetch(`${API}/resource/${resourceId}`);
    if (!response.ok) throw new Error("Could not load bookings.");
    setBookings(await response.json());
  }

  async function createBooking(event) {
    event.preventDefault();
    setMessage("");
    const response = await fetch(API, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ resourceId, userId, startDateTime: `${start}:00Z`, endDateTime: `${end}:00Z` })
    });
    const body = await response.json().catch(() => ({}));
    if (!response.ok) return setMessage(body.error ?? "The booking could not be created.");
    setMessage("Booking created.");
    await loadBookings();
  }

  async function cancelBooking(id) {
    const response = await fetch(`${API}/${id}/cancel`, { method: "POST" });
    if (response.ok) {
      setMessage("Booking cancelled.");
      await loadBookings();
    }
  }

  return <main className="shell">
    <header><p className="eyebrow">APPSWAVE / BOOKING DESK</p><h1>Reserve the right room.</h1><p className="subtitle">A focused booking workspace for shared resources.</p></header>
    <section className="grid">
      <form className="card form" onSubmit={createBooking}><h2>New booking</h2>
        <label>Resource<input value={resourceId} onChange={e => setResourceId(e.target.value)} required /></label>
        <label>User<input value={userId} onChange={e => setUserId(e.target.value)} required /></label>
        <div className="row"><label>Starts<input type="datetime-local" value={start} onChange={e => setStart(e.target.value)} required /></label><label>Ends<input type="datetime-local" value={end} onChange={e => setEnd(e.target.value)} required /></label></div>
        <button type="submit">Create booking</button>{message && <p className="message">{message}</p>}
      </form>
      <section className="card list"><div className="list-head"><div><p className="eyebrow">RESOURCE</p><h2>{resourceId}</h2></div><button className="secondary" onClick={loadBookings}>Refresh</button></div>
        {bookings.length === 0 ? <p className="empty">No bookings loaded yet.</p> : bookings.map(booking => <article className="booking" key={booking.id}><div><strong>{new Date(booking.startDateTime).toLocaleString()}</strong><span>to {new Date(booking.endDateTime).toLocaleString()} · {booking.userId}</span></div><div><span className={`status ${booking.status.toLowerCase()}`}>{booking.status}</span>{booking.status === "Active" && <button className="link" onClick={() => cancelBooking(booking.id)}>Cancel</button>}</div></article>)}
      </section>
    </section>
  </main>;
}

createRoot(document.getElementById("root")).render(<StrictMode><App /></StrictMode>);
