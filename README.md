# Arcade Nusantara

A mobile game project focused on preserving and modernizing traditional Indonesian games. Built with **Unity** for Android low-spec devices.

---

## 🎮 Overview

**Arcade Nusantara** is a cultural mini-game collection featuring classic Indonesian traditional games reimagined for mobile play. The project emphasizes:

* Lightweight performance for low-end Android devices
* Simple, intuitive touch controls
* Modular mini-game architecture
* Authentic representation of Indonesian cultural heritage

---

## 📌 Features (Current)

### ✅ Core Systems Implemented

* Mobile touch joystick
* Player controller (optimized for low-end devices)
* Lightweight AI system
* Object pooling framework
* Simplified spawning system
* Basic UI manager
* Global game state manager

### 🎲 Mini-Games

#### **1. Congklak (Added: Build 23 November 2025)**

The first completed mini-game module. Includes:

* Singleplayer mode vs AI
* Accurate seed distribution system
* Valid move checker
* Turn-based logic
* Collection animation and scoring
* Optimized board interactions for low-spec devices

---

## 🚀 Build History

### **Build 23 November 2025**

**Update:** Added complete Congklak gameplay module.

* Game board implementation
* Cup & store logic
* AI opponent baseline logic
* Touch interaction system
* Seamless integration with core game framework
* Initial UI for Congklak mode
* Performance pass for mobile

---

## 🏗 Project Structure

```
/ArcadeNusantara
  /Scripts
  /Assets
  /Resources
  /Scenes
```

---

## 📱 Mobile Optimization Principles

This project follows strict low-spec guidelines:

* Zero allocations in Update
* Object pooling for all repeatable objects
* No LINQ in runtime code
* Reduced texture sizes
* Limited physics usage
* Static batching and texture atlasing
* URP with stripped features

---

## 🔧 Tech Stack

* **Engine:** Unity 6.2
* **Language:** C#
* **Backend:** Firebase (planned)
* **AR:** AR Foundation (planned)
* **Multiplayer:** Photon Fusion (planned)
* **Version Control:** Git + Git LFS

---

## 📄 License

All code in this repository is property of the Arcade Nusantara development team. Redistribution is prohibited without explicit permission.

---

## 👥 Contributors

* **Lead Game Programmer:** Jasmine Unochi
* **2D Artist:** (to be added)
* **Technical Artist:** (to be added)
* **Producer:** (to be added)

---

## 📬 Contact

For development or collaboration inquiries, please reach out via email at jasmineunochii@gmail.com.
