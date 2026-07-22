             SELECT u.UnitNumber,
	            b.StartDate,
	            b.EndDate,
	            g.FirstName
            FROM Units AS u
            LEFT JOIN Bookings AS b ON b.UnitId = u.UnitId
	            AND b.StartDate <= CAST(GETDATE() AS DATE)
	            AND b.EndDate > CAST(GETDATE() AS DATE)
            LEFT JOIN Guests AS g ON g.GuestId = b.GuestId
            ORDER BY u.UnitNumber;