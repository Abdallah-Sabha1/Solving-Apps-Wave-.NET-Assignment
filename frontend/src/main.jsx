import { StrictMode, useEffect, useState } from "react";
import { createRoot } from "react-dom/client";
import "./styles.css";

const API = import.meta.env.VITE_API_URL ?? "http://localhost:5295/api/bookings";

function localDateTime(hoursFromNow) {
  const value = new Date(Date.now() + hoursFromNow * 60 * 60 * 1000);
  value.setMinutes(0, 0, 0);
  const offset = value.getTimezoneOffset() * 60_000;
  return new Date(value.getTime() - offset).toISOString().slice(0, 16);
}

function App() {
  const [resourceId, setResourceId] = useState("room-1");
  const [userId, setUserId] = useState("user-1");
  const [start, setStart] = useState(() => localDateTime(1));
  const [end, setEnd] = useState(() => localDateTime(2));
  const [bookings, setBookings] = useState([]);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function loadBookings() {
    if (!resourceId.trim()) return;
    setLoading(true);
    setError("");
    try {
      const response = await fetch(`${API}/resource/${encodeURIComponent(resourceId.trim())}`);
      if (!response.ok) throw new Error("Could not load bookings.");
      setBookings(await response.json());
    } catch {
      setError("Cannot reach the API. Make sure the backend is running on http://localhost:5295.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadBookings();
  }, []);

  async function createBooking(event) {
    event.preventDefault();
    setMessage("");
    setError("");
    setLoading(true);
    try {
      const response = await fetch(API, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          resourceId: resourceId.trim(),
          userId: userId.trim(),
          startDateTime: new Date(start).toISOString(),
          endDateTime: new Date(end).toISOString()
        })
      });
      const body = await response.json().catch(() => ({}));
      if (!response.ok) throw new Error(body.error ?? "The booking could not be created.");
      setMessage("Booking created successfully.");
      await loadBookings();
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setLoading(false);
    }
  }

  async function cancelBooking(id) {
    setError("");
    try {
      const response = await fetch(`${API}/${id}/cancel`, { method: "POST" });
      if (!response.ok) throw new Error("The booking could not be cancelled.");
      setMessage("Booking cancelled.");
      await loadBookings();
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  return <>
    <nav className="nav"><div className="brand"><span className="brand-mark">A</span><span>AppsWave</span></div><span className="nav-label">Booking management</span></nav>
    <main className="shell">
      <header><span className="badge">Shared resources</span><h1>Book a resource.</h1><p className="subtitle">Create, review, and cancel bookings from one simple workspace.</p></header>
      {(message || error) && <div className={`notice ${error ? "error" : "success"}`}>{error || message}</div>}
      <section className="grid">
        <form className="panel form" onSubmit={createBooking}>
          <div className="panel-heading"><div><h2>New booking</h2><p>Choose a resource and time range.</p></div></div>
          <label>Resource ID<input value={resourceId} onChange={event => setResourceId(event.target.value)} placeholder="room-1" required /></label>
          <label>User ID<input value={userId} onChange={event => setUserId(event.target.value)} placeholder="user-1" required /></label>
          <div className="row"><label>Start time<input type="datetime-local" value={start} onChange={event => setStart(event.target.value)} required /></label><label>End time<input type="datetime-local" value={end} onChange={event => setEnd(event.target.value)} required /></label></div>
          <button className="primary" type="submit" disabled={loading}>{loading ? "Working..." : "Create booking"}</button>
        </form>
        <section className="panel list">
          <div className="panel-heading list-heading"><div><h2>Bookings</h2><p>Showing bookings for <strong>{resourceId || "this resource"}</strong>.</p></div><button className="secondary" type="button" onClick={loadBookings} disabled={loading}>Refresh</button></div>
          {loading && bookings.length === 0 ? <p className="empty">Loading bookings...</p> : bookings.length === 0 ? <div className="empty"><span className="empty-icon">0</span><strong>No bookings yet</strong><p>Create the first booking for this resource.</p></div> : <div className="booking-list">{bookings.map(booking => <article className="booking" key={booking.id}><div className="booking-date"><span>{new Date(booking.startDateTime).toLocaleDateString([], { month: "short", day: "numeric" })}</span><strong>{new Date(booking.startDateTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</strong></div><div className="booking-details"><strong>{booking.resourceId}</strong><span>{booking.userId} · until {new Date(booking.endDateTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</span></div><div className="booking-actions"><span className={`status ${String(booking.status).toLowerCase()}`}>{booking.status}</span>{booking.status === "Active" && <button className="danger" type="button" onClick={() => cancelBooking(booking.id)}>Cancel</button>}</div></article>)}</div>}
        </section>
      </section>
    </main>
  </>;
}

createRoot(document.getElementById("root")).render(<StrictMode><App /></StrictMode>);
