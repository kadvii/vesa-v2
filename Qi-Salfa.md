# Qi-Salfa — Digital Rotating Savings System
### A Mini App Inside Super Qi (Qi Card)

---

## 1. Concept Overview

Qi-Salfa is a fully automated digital Jam'iya (rotating savings group) built as a mini app inside the Super Qi platform. Users pool monthly installments, and each month one member receives the full collected amount. Payment is automatically deducted from salary before the user receives it, eliminating default risk entirely.

---

## 2. User Source & Authentication

- All users come exclusively through the **Super Qi app**
- Super Qi provides only **verified salary employees** with no account issues
- No separate registration or login required
- System pulls from Super Qi API:
  - `user_id`
  - `salary_amount`
  - `account_status`
- Permissions and eligibility are assigned automatically based on this data

**Result:** No guarantor system. No personal accounts. No manual verification.

---

## 3. Business Rules

### 40% Rule
- A user's total monthly installments across **all active loans** must not exceed 40% of their salary
- Checked in real-time every time a user attempts to join a new group
- Formula: `total_active_installments / salary ≤ 0.40`

### Available Capacity Display
- Users see a live indicator: *"You have X IQD/month available for new loans"*
- Only groups whose installment fits within remaining capacity are shown to the user
- Prevents applications that would be rejected

### Salary Change Handling
- Salary increase → capacity increases, user can join new groups
- Salary decrease → existing commitments are grandfathered and honored, no new loans until back in compliance
- Active cycles are never interrupted mid-way

### Multiple Loans
- A user can join multiple groups simultaneously
- Total installments across all groups must remain ≤ 40% of salary

---

## 4. Loan Amounts & Durations

### Available Amounts (IQD)
- 500,000
- 1,000,000
- 5,000,000
- 10,000,000
- 15,000,000
- 20,000,000
- 25,000,000

### Available Durations
- 10 months (10 members per group)
- 18 months (18 members per group)
- 24 months (24 members per group)

### Installment Matrix (Monthly Payment per Member)

| Group Amount | 10 months | 18 months | 24 months |
|---|---|---|---|
| 500,000 IQD | 50,000/mo | 27,778/mo | 20,833/mo |
| 1,000,000 IQD | 100,000/mo | 55,556/mo | 41,667/mo |
| 5,000,000 IQD | 500,000/mo | 277,778/mo | 208,333/mo |
| 10,000,000 IQD | 1,000,000/mo | 555,556/mo | 416,667/mo |
| 15,000,000 IQD | — | 833,333/mo | 625,000/mo |
| 20,000,000 IQD | — | — | 833,333/mo |
| 25,000,000 IQD | — | — | 1,041,667/mo |

> Combinations marked — are excluded because the installment exceeds the realistic salary range of most government employees.

### Phase 1 Launch Groups (Recommended)
- 500,000 IQD / 10 months → 50,000 IQD/mo
- 1,000,000 IQD / 10 months → 100,000 IQD/mo
- 5,000,000 IQD / 18 months → 277,778 IQD/mo

---

## 5. Group Logic

- Number of members = number of months in the cycle
- Each group is an independent unit
- Installment = Total Amount ÷ Duration
- When a group fills, the lottery process begins
- If demand exceeds available spots, the system automatically opens a new group of the same type

---

## 6. Lottery System

All slot ordering is determined by a fully random, verifiable lottery. No paid priority. No manual ordering.

### Process:
1. Group reaches full capacity
2. System generates an **encrypted random seed** (stored publicly and permanently)
3. Lottery assigns each member a month number (1 through N)
4. Results are sent to all members
5. Members have **48 hours** to confirm or exit
6. Any member who exits is replaced; remaining open slots re-run the lottery
7. Final schedule is locked and sent to all members:
   - Their position
   - Their receiving month
   - Full payment schedule

### Transparency:
- The lottery hash is publicly viewable
- Any member can independently verify the result was not manipulated

---

## 7. Payment System

- Deduction is automatic via Qi Card salary pipeline
- Deducted **before** salary reaches the user's account

### Notification Timeline:
| Trigger | Notification |
|---|---|
| 7 days before deduction | Reminder to user |
| Deduction day | Confirmation of upcoming deduction |
| Successful deduction | Receipt to user |
| Failed deduction | Alert to user + alert to admin |

### Failed Deduction Handling:
1. Notify user immediately
2. Notify admin
3. If unresolved → draw from emergency fund to cover that month
4. Admin follows up with user

---

## 8. Emergency Fund

- **1% of every installment payment** is automatically redirected to the emergency fund
- Fund is used exclusively to cover temporary failed deductions
- Fund balance is visible on the admin dashboard at all times
- Replenished continuously as long as groups are active

---

## 9. Revenue Model

**1% transaction fee added to every installment transfer.**

- The fee is added **on top** of the installment — the recipient always receives the full promised amount
- Each paying member pays: `installment + 1% of installment`

### Example:
> Group: 1,000,000 IQD | 10 members | 10 months  
> Monthly installment: 100,000 IQD  
> Fee per payment: 1,000 IQD  
> Platform revenue per group: 10 members × 10 months × 1,000 IQD = **100,000 IQD**

Revenue scales directly with the number of active groups and the size of installments — no flat fees, no subscription charges.

---

## 10. Two Separate Interfaces

### Mini App — Inside Super Qi (User-Facing)

Lightweight, embedded in Super Qi. Users never leave the main app.

**Screens:**

**Home**
- Active loans overview
- Upcoming deduction date and amount
- Available salary capacity remaining

**Browse Groups**
- List of open groups the user qualifies for (auto-filtered by 40% capacity)
- Each group shows: amount, duration, monthly installment, spots remaining
- One-tap join flow with confirmation screen

**My Loans**
- All active and completed cycles
- Per loan: full schedule, lottery position, receiving month, payment history

**Notifications**
- Deduction reminders and confirmations
- Lottery results
- Group status updates

---

### Dashboard — Separate Web App (Admin-Facing)

Full management and monitoring interface for Qi staff.

**Sections:**

**Overview**
- Active groups count
- Total money in circulation
- Revenue collected this month
- Emergency fund balance
- Open defaults count

**Groups Management**
- Create new group (set amount + duration)
- View all groups: active, pending, completed
- Per-group view: member list, payment status per member, lottery result
- Freeze or close a group if needed

**Advances (New Group Creation)**
- Admin initiates a new advance offering
- System opens it for users to join via mini app
- Dashboard tracks fill rate in real-time

**Users**
- Search any user
- View: active loans, salary on record, capacity used, payment history
- Flag or restrict a user if needed

**Defaults & Issues**
- List of all failed deductions
- Status: pending, resolved via emergency fund, escalated
- Action log per default case

**Finance**
- Revenue breakdown per group and time period
- Emergency fund inflow/outflow history
- Exportable reports

---

## 11. System Architecture

```
┌─────────────────────────────┐
│        Super Qi App         │
│  ┌──────────────────────┐   │
│  │   Qi-Salfa Mini App  │   │
│  └──────────┬───────────┘   │
└─────────────┼───────────────┘
              │
         [Backend API]
              │
    ┌─────────┴──────────┐
    │                    │
[Database]        [Admin Dashboard]
                  (Separate Web App)
```

- One shared backend and database
- Mini App and Dashboard are two separate frontends consuming the same API
- Super Qi API provides user identity and salary data

---

## 12. Roadmap

### Phase 1 — Foundation
- Salary employees only (via Super Qi)
- 3 group types (500K/10mo, 1M/10mo, 5M/18mo)
- Full lottery system with encrypted verification
- Auto-deduction and notification system
- Basic admin dashboard

### Phase 2 — Expansion
- All loan amount and duration combinations unlocked
- Multi-loan support with live capacity tracking
- Emergency fund automation fully live
- Enhanced admin reporting and finance export

### Phase 3 — Platform Growth
- Salfa Score: internal credit score built from payment history
- High-score users get early access to newly opened groups
- B2B offering: companies can run internal employee Jam'iyas
- Bridge loan product: Qi advances the full amount upfront for high-score users

---

## 13. Summary

| Element | Decision |
|---|---|
| User source | Super Qi API — pre-verified salary employees only |
| Authentication | No login — identity from Super Qi |
| Group ordering | Full random lottery, encrypted and publicly verifiable |
| Revenue | 1% fee added on top of each installment transfer |
| Risk control | Real-time 40% debt-to-income check across all active loans |
| Default protection | 1% emergency fund from every installment |
| Interfaces | Mini App (inside Super Qi) + Admin Dashboard (separate web app) |
| Phase 1 scope | 3 group types, 10-month cycles, employees only |

---

*Qi-Salfa — Making the traditional Jam'iya automatic, fair, and default-proof.*
