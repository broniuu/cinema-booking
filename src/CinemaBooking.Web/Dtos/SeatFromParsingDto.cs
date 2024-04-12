﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Seed.Dtos;
public class SeatFromParsingDto
{
    public Guid Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public required string SeatNumber { get; set; }
}
